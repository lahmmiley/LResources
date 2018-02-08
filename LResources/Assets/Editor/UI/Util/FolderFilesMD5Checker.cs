using EditorTools.AssetBundle;
using EditorTools.Patch;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

//完全一致
namespace EditorTools.UI
{
    /// <summary>
    /// 检查图集的图片列表是否改变了，若改变了，则需要重新生成图集
    /// 避免重新生成图集
    /// </summary>
    public class FolderFilesMD5Checker
    {
        public static bool IsFilesChanged(string folderPath)
        {
            Md5Record record = ReadMd5Record(folderPath);
            Dictionary<string, string> newValue = ComputeMd5(folderPath);
            Dictionary<string, string> oldValue = ReadMd5(record);
            bool isMd5Changed = IsMd5Changed(newValue, oldValue);
            if(isMd5Changed)
            {
                WriteMd5Record(newValue, folderPath, record);
            }
            return isMd5Changed;
        }

        private static bool IsMd5Changed(Dictionary<string, string> newValue, Dictionary<string, string> oldValue)
        {
            bool result = false;
            if (newValue.Keys.Count != oldValue.Keys.Count)
            {
                result = true;
            }
            else
            {
                foreach (string key in newValue.Keys)
                {
                    if (!oldValue.Keys.Contains(key))
                    {
                        result = true;
                        break;
                    }
                    if (newValue[key] != oldValue[key])
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, string> ComputeMd5(string folderPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string systemPath = UIPrefabProcessor.ToFileSystemPath(folderPath);
            string[] allFilePaths = Directory.GetFiles(systemPath, "*.*");
            List<string> filePathList = new List<string>();
            foreach (string s in allFilePaths)
            {
                if (s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".png.meta"))
                {
                    filePathList.Add(s);
                }
            }
            //MD5 md5 = MD5.Create();
            for (int i = 0; i < filePathList.Count; i++)
            {
                string path = filePathList[i];
                //byte[] data = md5.ComputeHash(File.ReadAllBytes(path));
                //result.Add(UIPrefabProcessor.ToAssetPath(path), Convert.ToBase64String(data));
                result.Add(UIPrefabProcessor.ToAssetPath(path), AssetPatchMaker.GetFileLastWriteTime(path));
            }
            return result;
        }

        private static void WriteMd5Record(Dictionary<string, string> value, string folderPath, Md5Record record)
        {
            if(record == null)
            {
                record = new Md5Record();
                record.value = new Dictionary<string, List<string>>();
            }
            string buildTarget = AssetPathHelper.GetBuildTarget(AssetPathHelper.GetBuildTarget());
            List<string> tokenList = new List<string>();
            foreach(string key in value.Keys)
            {
                tokenList.Add(key);
                tokenList.Add(value[key]);
            }
            if(record.value.Keys.Contains(buildTarget))
            {
                record.value[buildTarget] = tokenList;
            }
            else
            {
                record.value.Add(buildTarget, tokenList);
            }
            string content = JsonMapper.ToJson(record);
            content = content.Replace("\"Assets/Things/", "\n\"Assets/Things/");
            string path = GetMd5RecordPath(folderPath);
            WriteJson(content, path);
        }

        private static void WriteJson(string content, string path)
        {
            string jsonPath = UIPrefabProcessor.ToFileSystemPath(path);
            if (File.Exists(jsonPath) == true)
            {
                File.Delete(jsonPath);
            }
            if (!string.IsNullOrEmpty(content))
            {
                StreamWriter sw = File.CreateText(jsonPath);
                sw.Write(content);
                sw.Close();
                AssetDatabase.ImportAsset(path);
            }
        }

        /// <summary>
        /// 返回值 Key为资源路径，Value为资源Md5值,资源包括meta文件
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ReadMd5(Md5Record record)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (record != null)
            {
                string buildTarget = AssetPathHelper.GetBuildTarget(AssetPathHelper.GetBuildTarget());
                if (record.value.Keys.Contains(buildTarget))
                {
                    List<string> tokenList = record.value[buildTarget];
                    for (int i = 0; i < tokenList.Count; i += 2)
                    {
                        result.Add(tokenList[i], tokenList[i + 1]);
                    }
                }
            }
            return result;
        }

        private static Md5Record ReadMd5Record(string folderPath)
        {
            Md5Record record = null;
            string path = GetMd5RecordPath(folderPath);
            TextAsset jsonAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            if(jsonAsset != null)
            {
                record = JsonMapper.ToObject<Md5Record>(jsonAsset.text);
            }
            return record;
        }

        /// <summary>
        /// 图片文件列表的Md5保存在图集文件夹下
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static string GetMd5RecordPath(string folderPath)
        {
            return UIPrefabProcessor.GetShadowTextureFolderPath(folderPath) + "/md5.json";
        }
    }

    /// <summary>
    /// Md5记录序列化数据结构
    /// {
    ///     "android":["filePath0", "fileMd50", "filePath1", "fileMd5：1"]
    /// }
    /// </summary>
    public class Md5Record
    {
        public Dictionary<string, List<string>> value;
    }
}

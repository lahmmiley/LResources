using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace EditorTools.UI
{
    public class AtlasGenerator
    {
        public const int ATLAS_MAX_SIZE = 2048;

        public static string[] Generate(string[] folderPaths)
        {
            string[] result = new string[folderPaths.Length];
            for(int i = 0; i < folderPaths.Length; i++)
            {
                string folderPath = folderPaths[i];
                string[] assetPaths = GetAssetPaths(folderPath);
                string tag = GetFolderTexturePackingTag(assetPaths);
            }
            return result;
        }

        /// <summary>
        /// 获取目录下Texture的Packingtag
        /// 同时验证同一目录下Texture的PackingTag是否相同，不同则报错，提示修改
        /// </summary>
        /// <param name="assetPaths"></param>
        /// <returns></returns>
        private static string GetFolderTexturePackingTag(string[] assetPaths)
        {
            string tag = null;
            foreach(string s in assetPaths)
            {
                string nextTag = GetTexturePackingTag(s);
                if(tag != null && tag != nextTag)
                {
                    UIPrefabProcessor.ThrowException("同一目录下存在不同的SpritePackingTag，路径： " + s + " tag: "  + nextTag);
                }
                else
                {
                    tag = nextTag;
                }
            }
        }

        private static string GetTexturePackingTag(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            return importer.spritePackingTag;
        }

        private static string[] GetAssetPaths(string folderPath)
        {
            string systemPath = UIPrefabProcessor.ToFileSystemPath(folderPath);
            string[] filePaths = Directory.GetFiles(systemPath, "*.png");
            string[] result = new string[filePaths.Length];
            for(int i = 0; i < filePaths.Length; i++)
            {
                result[i] = UIPrefabProcessor.ToAssetPath(filePaths[i]);
            }
            return result;
        }
    }
}

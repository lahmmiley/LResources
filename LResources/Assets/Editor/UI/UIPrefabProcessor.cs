using EditorTools.AssetBundle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.UI
{
    public class UIPrefabProcessor
    {
        /// <summary>
        /// UIPrefab根路径
        /// </summary>
        public static string UI_PREFAB_ROOT = "Assets/Things/Prefabs/UI/";

        /// <summary>
        /// UIPrefab副本根路径
        /// </summary>
        public static string UI_PREFAB_ROOT_SHADOW = "Assets/Things/Prefabs/UI_{0}";

        /// <summary>
        /// UIPrefab中单张图片列表目录根路径
        /// </summary>
        public static string UI_TEXTURE_ROOT = "Assets/Things/Texture/UI/";

        public static string Process(string prefabPath)
        {
            //获得Prefab依赖的单张图片列表的目录，可能存在多个目录
            string[] textureFolderPaths = GetPrefabDependentTextureFolderPaths(prefabPath);
            //将每一个图片目录做成一个图集
            string[] atlasPaths = AtlasGenerator.Generate(textureFolderPaths);
        }

        private static string[] GetPrefabDependentTextureFolderPaths(string path)
        {
            string[] paths = AssetDatabase.GetDependencies(path);
            HashSet<string> result = new HashSet<string>();
            foreach(string s in paths)
            {
                if(s.Contains(IconProcessor.ICON_OUT_ROOT))
                {
                    throw new Exception("UI预设中不可以引用Icon资源[" + path + "]=>[" + s + "]");
                }
                if(s.ToLower().EndsWith(".png") == true)
                {
                    result.Add(GetFolderPath(s));
                }
            }
            return result.ToArray<string>();
        }

        public static string GetFolderPath(string path)
        {
            int index = path.LastIndexOf("/");
            return path.Substring(0, index);
        }

        public static string ToFileSystemPath(string assetPath)
        {
            return Application.dataPath.Replace("Assets", "") + assetPath;
        }

        public static string ToAssetPath(string systemPath)
        {
            systemPath = systemPath.Replace("\\", "/");
            return "Assets" + systemPath.Substring(Application.dataPath.Length);
        }

        public static string GetShadowTextureFolderPath(string folderPath)
        {
            return folderPath.Replace(UI_TEXTURE_ROOT, GetShadowTextureFolderRoot());
        }

        public static string GetShadowTextureFolderRoot()
        {
            return string.Format(UI_PREFAB_ROOT_SHADOW, AssetPathHelper.GetBuildTarget(AssetPathHelper.GetBuildTarget()));
        }

        public static void ThrowException(string msg)
        {
            EditorUtility.DisplayDialog("错误", msg, "马上调整Go~");
            throw new Exception(msg);
        }

        public static void CreateInexistentFolder(string path)
        {
            string folderPath = Path.GetDirectoryName(ToFileSystemPath(path));
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}

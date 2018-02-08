using EditorTools.AssetBundle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

//基本一致
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
            //将Prefab复制出来一个副本
            string copyPath = CopyPrefab(prefabPath);
            //将副本Prefab中Image组件上的资源依赖重定向到图集的Sprite
            ReplaceImageSprite(copyPath, atlasPaths);
            return copyPath;
        }

        public static string GetFolderPath(string path)
        {
            int index = path.LastIndexOf("/");
            return path.Substring(0, index);
        }

        private static string CopyPrefab(string source)
        {
            string target = GetCopyPrefabPath(source);
            CreateInexistentFolder(target);
            AssetDatabase.CopyAsset(source, target);
            return target;
        }

        private static void ReplaceImageSprite(string prefabPath, string[] atlasPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            Dictionary<string, List<Sprite>> spriteListDict = GetSpriteListDict(atlasPaths);
            Dictionary<string, Material> materialDict = GetMaterialDict(atlasPaths);
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Image[] images = go.GetComponentsInChildren<Image>(true);
            foreach(Image image in images)
            {
                Sprite sprite = image.sprite;
                if(sprite != null)
                {
                    ChangeImageSpriteMaterial(image, spriteListDict, materialDict);
                }
            }
            PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ConnectToPrefab);
            Object.DestroyImmediate(go, true);
        }

        private static Dictionary<string, List<Sprite>> GetSpriteListDict(string[] atlasPaths)
        {
            Dictionary<string, List<Sprite>> result = new Dictionary<string, List<Sprite>>();
            for (int i = 0; i < atlasPaths.Length; i++)
            {
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath(atlasPaths[i]);
                List<Sprite> spriteList = new List<Sprite>();
                foreach (Object o in objs)
                {
                    if (o is Sprite)
                    {
                        spriteList.Add(o as Sprite);
                    }
                }
                result.Add(atlasPaths[i], spriteList);
            }
            return result;
        }

        private static Dictionary<string, Material> GetMaterialDict(string[] atlasPaths)
        {
            Dictionary<string, Material> result = new Dictionary<string, Material>();
            foreach (string s in atlasPaths)
            {
                result.Add(s, AssetDatabase.LoadAssetAtPath(AtlasGenerator.GetMaterialPath(s), typeof(Material)) as Material);
            }
            return result;
        }

        private static void ChangeImageSpriteMaterial(Image image, Dictionary<string, List<Sprite>> spriteListDict, Dictionary<string, Material> materialDict)
        {
            Sprite sprite = image.sprite;
            string path = AssetDatabase.GetAssetPath(sprite);
            string folderPath = GetFolderPath(path);
            string tag = (AssetImporter.GetAtPath(path) as TextureImporter).spritePackingTag;
            string atlasPath = AtlasGenerator.GetAtlasPath(folderPath, tag);
            List<Sprite> spriteList = spriteListDict[atlasPath];
            foreach(Sprite s in spriteList)
            {
                if(s.name == sprite.name)
                {
                    image.sprite = s;
                    image.material = materialDict[atlasPath];
                    break;
                }
            }
        }

        public static void CreateInexistentFolder(string path)
        {
            string folderPath = Path.GetDirectoryName(ToFileSystemPath(path));
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private static string GetCopyPrefabPath(string sourcePath)
        {
            return sourcePath.Replace(UI_PREFAB_ROOT, GetShadowTextureFolderRoot());
        }

        public static string GetShadowTextureFolderPath(string folderPath)
        {
            return folderPath.Replace(UI_TEXTURE_ROOT, GetShadowTextureFolderRoot());
        }

        public static string GetShadowTextureFolderRoot()
        {
            return string.Format(UI_PREFAB_ROOT_SHADOW, AssetPathHelper.GetBuildTarget(AssetPathHelper.GetBuildTarget()));
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

        public static string ToFileSystemPath(string assetPath)
        {
            return Application.dataPath.Replace("Assets", "") + assetPath;
        }

        public static string ToAssetPath(string systemPath)
        {
            systemPath = systemPath.Replace("\\", "/");
            return "Assets" + systemPath.Substring(Application.dataPath.Length);
        }

        public static void ThrowException(string msg)
        {
            EditorUtility.DisplayDialog("错误", msg, "马上调整Go~");
            throw new Exception(msg);
        }
    }
}

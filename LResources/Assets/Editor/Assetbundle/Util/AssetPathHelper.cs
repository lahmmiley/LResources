using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle{
    public class AssetPathHelper
    {
        public const string PATH_RESOURCES = "Assets/Things/";
        public const string PATH_RESOURCES_TEMP = "Assets/Things_temp/";
        public const string POSTFIX_SINGLE = ".single";
        public const string POSTFIX_FOLDER = ".folder";
        public const string POSTFIX_SELECTION = ".selection";
        public const string FBX = ".fbx";
        public const string TTF = ".ttf";

        public static BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        public static string patchVersion = null;

        private const string TOKEN_ASSETS = "Assets";
        /// <summary>
        /// folder模式打包中，正则表达式中必须定义最终生成AB文件路径名的子模式
        /// </summary>
        public const string REGEX_TOKEN_PATH = "path";

        public static Dictionary<BuildTarget, string> GetBuildTargetIdentifierDict()
        {
            Dictionary<BuildTarget, string> result = new Dictionary<BuildTarget, string>();
            result.Add(BuildTarget.Android, "android");
            result.Add(BuildTarget.StandaloneWindows, "pc");
            result.Add(BuildTarget.StandaloneWindows64, "pc");
            result.Add(BuildTarget.StandaloneOSXIntel, "pc");
            result.Add(BuildTarget.StandaloneOSXIntel64, "pc");
            result.Add(BuildTarget.iOS, "ios");
            return result;
        }

        public static string ToFileSystemPath(string assetPath)
        {
            return Application.dataPath.Replace(TOKEN_ASSETS, "") + assetPath;
        }

        public static BuildTarget GetBuildTarget()
        {
            return buildTarget;
        }

        public static string GetBuildTarget(BuildTarget pbuildTarget)
        {
            switch( pbuildTarget)
            {
                case BuildTarget.Android:
                    return "android";
                case BuildTarget.StandaloneWindows:
                    return "pc";
                case BuildTarget.StandaloneWindows64:
                    return "pc";
                case BuildTarget.StandaloneOSXIntel:
                    return "pc";
                case BuildTarget.StandaloneOSXIntel64:
                    return "pc";
                case BuildTarget.iOS:
                    return "ios";
                default:
                    throw new Exception("无法识别平台" + pbuildTarget.ToString());
            }
        }

        /// <summary>
        /// entryPath:打包入口资源路径
        /// assetPath:打包入口资源所依赖的资源路径
        /// </summary>
        /// <param name="entryPath"></param>
        /// <param name="materialPath"></param>
        /// <param name="material"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetObjectKey(string entryPath, string assetPath, Object obj, StrategyNode node)
        {
            string token = string.Empty;
            if(string.IsNullOrEmpty(assetPath))
            {
                throw new Exception("Asset physicalPath should not be empty!");
            }

            if(node.mode == PackageMode.SINGLE)
            {
                token = GetSingleModeBundlePath(assetPath);
                //字体ttf和模型文件是特殊的类型，一个Asset中包含多个Object
                //AssetDatabase.LoadAllAssetsAtPath(assetPath) 返回长度大于一
                if(token.Contains(FBX) || token.Contains(TTF))
                {
                    token = token + obj.GetType().Name + obj.name;
                }
            }
            else if(node.mode == PackageMode.FOLDER)
            {
                token = GetFolderModeBundlePath(assetPath, node.pattern) + obj.GetType().Name + obj.name;
            }
            else if(node.mode == PackageMode.SELECTION)
            {
                token = GetSelectionModeBundlePath(entryPath, assetPath, node.pattern) + obj.GetType().Name + obj.name;
            }
            return token;
        }

        private static string GetSelectionModeBundlePath(string entryPath, string assetPath, Regex pattern)
        {
            string result = GetSelectionModeStartPath(entryPath);
            string postfix = GetPostfix(assetPath, pattern);
            return ReplaceSlash(EliminaterStartToken(result + postfix + POSTFIX_SELECTION)).ToLower();
        }

        /// <summary>
        /// selection模式下资源包路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSelectionModeStartPath(string path)
        {
            return path.Substring(0, path.LastIndexOf("."));
        }

        private static string GetFolderModeBundlePath(string assetPath, Regex pattern)
        {
            string result = GetFolderModeBundlePath(assetPath, pattern);
            string postfix = GetPostfix(assetPath, pattern);
            return ReplaceSlash(EliminaterStartToken(result + postfix + POSTFIX_FOLDER)).ToLower();
        }

        private static string GetPostfix(string path, Regex pattern)
        {
            GroupCollection gc = pattern.Match(path).Groups;
            string result = gc[REGEX_TOKEN_PATH].Value;
            if(string.IsNullOrEmpty(result))
            {
                return string.Empty;
            }
            return "." + result.ToLower();
        }

        public static string GetSingleModeBundlePath(string path)
        {
            return (ReplaceSlash(EliminaterStartToken(path) + POSTFIX_SINGLE)).ToLower().Replace(" ", "");
        }

        public static string ReplaceSlash(string path)
        {
            return path.Replace("/", "$");
        }

        public static string EliminaterStartToken(string path)
        {
            if(path.StartsWith(PATH_RESOURCES))
            {
                return path.Substring(PATH_RESOURCES.Length);
            }
            if(path.StartsWith(PATH_RESOURCES_TEMP))
            {
                return path.Substring(PATH_RESOURCES_TEMP.Length);
            }
            return path;
        }

        public static string GetSelectionModeBundlePath(string entryPath, List<string> assetPathList, Regex pattern)
        {
            return GetSelectionModeBundlePath(entryPath, assetPathList[0], pattern).Replace(" ", "");
        }

        public static string GetFolderModeBundlePath(List<string> assetPathList, Regex pattern)
        {
            return GetFolderModeBundlePath(assetPathList[0], pattern).Replace(" ", "");
        }

        /// <summary>
        /// folder模式下的资源包路径
        /// </summary>
        /// <param name="p"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static string GetFolderModeStartPath(string path, Regex pattern)
        {
            GroupCollection gc = pattern.Match(path).Groups;
            string result = gc[REGEX_TOKEN_PATH].Value;
            int lastSlashIndex = result.LastIndexOf(@"/");
            if(lastSlashIndex == result.Length - 1)
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static string ToAssetPath(string systemPath)
        {
            return "Assets" + systemPath.Substring(Application.dataPath.Length);
        }

        /// <summary>
        /// 获取AB文件的输出路径
        /// </summary>
        /// <param name="bundlePath"></param>
        /// <returns></returns>
        public static string GetOutputPath(string assetbundleName)
        {
            string basePath = AssetBuildStrategyManager.outputPath;
            return basePath + assetbundleName;
        }
    }
}

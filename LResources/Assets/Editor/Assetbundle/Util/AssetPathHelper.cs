using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle{
    public class AssetPathHelper
    {
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

    }
}

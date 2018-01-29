using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace EditorTools.AssetBundle{
    public class AssetPathHelper
    {
        public static BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        public static string patchVersion = null;


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
    }
}

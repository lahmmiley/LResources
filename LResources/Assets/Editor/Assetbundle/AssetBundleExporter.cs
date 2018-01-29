using UnityEngine;
using System.Collections;

using Object = UnityEngine.Object;
using UnityEditor;
using System;

namespace EditorTools.AssetBundle
{
    public class AssetBundleExporter
    {
        internal const string LOGGER_NAME = "AssetBundle";

        public static void BuildFromSelection()
        {
            AssetPathHelper.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            AssetPathHelper.patchVersion = DateTime.Now.ToString("yyyyMMddHHmm");
            Initialize();
        }

        private static void Initialize()
        {
            //TemporaryAssetHelper.Initialize();
            //MeterialJsonData.Initialize();
            AssetBuildStrategyManager.Initialize();
        }

        public static void ThrowException(string msg)
        {
            EditorUtility.DisplayDialog("错误", msg, "马上调整Go~");
            throw new Exception(msg);
        }
    }
}



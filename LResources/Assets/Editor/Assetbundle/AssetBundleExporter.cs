using UnityEngine;
using System.Collections;

using Object = UnityEngine.Object;
using UnityEditor;
using System;

namespace EditorTools.AssetBundle
{
    public class AssetBundleExporter
    {
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
    }
}



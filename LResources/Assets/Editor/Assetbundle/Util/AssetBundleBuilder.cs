using EditorTools.AssetBundle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace EditorTools.Assetbundle
{
    public class AssetBundleBuilder
    {
        //Key为生成AssetBundle文件名，Value为与之对应的AssetBundleBuild定义
        private static Dictionary<string, AssetBundleBuilderWrapper> _assetBundleBuildDict;

        public static void Initialize()
        {
            _assetBundleBuildDict = new Dictionary<string, AssetBundleBuilderWrapper>();
        }
    }

    struct AssetBundleBuilderWrapper
    {
        public AssetBundleBuild bulid;
        public string entryPath;
        public string bundlePath;
        public StrategyNode node;
    }
}

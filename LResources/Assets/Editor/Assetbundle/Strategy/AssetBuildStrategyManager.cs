using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;

namespace EditorTools.AssetBundle
{
    public class AssetBuildStrategyManager
    {
        public const string SETTING_PATH = "Assets/build.xml";

        //缓存查询结果
        private static Dictionary<string, AssetBuildStrategy> _assetStrategyDict;
        //解析build.xml中定义的策略
        private static Dictionary<string, AssetBuildStrategy> _defineStrategyDict;

        public static void Initialize()
        {
            Dictionary<BuildTarget, string> buildTargetIdentifierDict = GetBuildTargetIndentifierDict();
            _assetStrategyDict = new Dictionary<string, AssetBuildStrategy>();
            _defineStrategyDict = new Dictionary<string, AssetBuildStrategy>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.Load(AssetPathHelper.ToFileSystemPath(SETTING_PATH));

        }

        private static Dictionary<BuildTarget, string> GetBuildTargetIndentifierDict()
        {
            return AssetPathHelper.GetBuildTargetIdentifierDict();
        }
    }
}

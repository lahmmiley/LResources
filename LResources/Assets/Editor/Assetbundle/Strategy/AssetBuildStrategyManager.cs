using EditorTools.Assetbundle;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static string outputPath;
        public static bool isSaveTemp;
        public static bool isBuild;
        public static bool isReport;
        public static bool isSaveUIMediate;

        public static void Initialize()
        {
            Dictionary<BuildTarget, string> buildTargetIdentifierDict = GetBuildTargetIndentifierDict();
            _assetStrategyDict = new Dictionary<string, AssetBuildStrategy>();
            _defineStrategyDict = new Dictionary<string, AssetBuildStrategy>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.Load(AssetPathHelper.ToFileSystemPath(SETTING_PATH));
            XmlNode root = xmlDoc.SelectSingleNode("root");
            outputPath = root.Attributes["output"].Value + buildTargetIdentifierDict[AssetPathHelper.GetBuildTarget()] + "/";
            CreateOutputFolder(outputPath);
            isSaveTemp = root.Attributes["saveTempFile"].Value.ToLower() == "true";
            isBuild = root.Attributes["build"].Value.ToLower() == "true";
            isReport = root.Attributes["report"].Value.ToLower() == "true";
            isSaveUIMediate = root.Attributes["saveUIMediate"].Value.ToLower() == "true";

            foreach (XmlNode node in root.ChildNodes)
            {
                if(!(node is XmlElement))
                {
                    continue;
                }
                AssetBuildStrategy strategy = new AssetBuildStrategy(node);
                if(string.IsNullOrEmpty(strategy.name) == true)
                {
                    throw new Exception("Build strategy name not set " + node.InnerText);
                }
                if(_defineStrategyDict.Keys.Contains(strategy.name))
                {
                    throw new Exception("Duplicated strategy name:" + strategy.name);
                }
                _defineStrategyDict.Add(strategy.name, strategy);
            }
        }

        private static void CreateOutputFolder(string path)
        {
            if(Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        private static Dictionary<BuildTarget, string> GetBuildTargetIndentifierDict()
        {
            return AssetPathHelper.GetBuildTargetIdentifierDict();
        }

        public static AssetBuildStrategy GetAssetBuildStrategy(string entryPath, bool showLog = true)
        {
            if(_assetStrategyDict.ContainsKey(entryPath))
            {
                return _assetStrategyDict[entryPath];
            }
            foreach(AssetBuildStrategy strategy in _defineStrategyDict.Values)
            {
                if(strategy.entryPattern.IsMatch(entryPath))
                {
                    if (showLog) Logger.GetLogger(AssetBundleExporter.LOGGER_NAME).Log(string.Format("<color=#0000ff>Path: {0} Matches Strategy: {1}</color>", entryPath, strategy.name));
                    _assetStrategyDict.Add(entryPath, strategy);
                    return strategy;
                }
            }
            if (showLog) Logger.GetLogger(AssetBundleExporter.LOGGER_NAME).Log(string.Format("<color=#0000ff>Path: {0} 未找到匹配的打包策略！</color>", entryPath));
            return null;
        }
    }
}

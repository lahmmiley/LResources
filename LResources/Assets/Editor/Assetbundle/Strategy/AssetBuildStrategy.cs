using EditorTools.Assetbundle;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace EditorTools.AssetBundle
{
    public class AssetBuildStrategy
    {
        public string name;
        public Regex entryPattern;
        public List<StrategyNode> nodeList;

        public AssetBuildStrategy(XmlNode xmlNode)
        {
            name = xmlNode.Attributes["name"].Value;
            entryPattern = new Regex(xmlNode.SelectSingleNode("path").InnerText, RegexOptions.IgnoreCase);
            nodeList = GetStrategyNodeList(xmlNode.SelectSingleNode("strategy").InnerText, name);
        }

        private List<StrategyNode> GetStrategyNodeList(string json, string strategyName)
        {
            List<StrategyNode> result = new List<StrategyNode>();
            JsonData jsonData = null;
            try
            {
                jsonData = JsonMapper.ToObject(json);
            }catch(Exception e)
            {
                Logger.GetLogger(AssetBundleExporter.LOGGER_NAME).Log(json);
                Logger.GetLogger(AssetBundleExporter.LOGGER_NAME).Exception(e);
            }

            JsonData strategyData = jsonData["strategy"];
            for(int i = 0; i < strategyData.Count; i++)
            {
                StrategyNode node = new StrategyNode();
                JsonData nodeData = strategyData[i];
                node.strategy = strategyName;
                node.processor = nodeData.Keys.Contains("processor") == true ? (string)nodeData["processor"] : string.Empty;
                node.mode = ((string)nodeData["mode"]).ToLower();
                node.pattern = new Regex((string)nodeData["pattern"], RegexOptions.IgnoreCase);
                result.Add(node);
                Verify(node);
            }
            
            return result;
        }

        private void Verify(StrategyNode node)
        {
            if(node.mode == PackageMode.FOLDER)
            {
                string[] groupName = node.pattern.GetGroupNames();
                int index = Array.IndexOf<string>(groupName, AssetPathHelper.REGEX_TOKEN_PATH);
                if(index == -1)
                {
                    string msg = "打包策略中，Folder模式节点中Pattern错误，没有文件夹路径定义，策略：" + node.strategy + "， Pattern" + node.pattern;
                    AssetBundleExporter.ThrowException(msg);
                }
            }
        }
    }
}

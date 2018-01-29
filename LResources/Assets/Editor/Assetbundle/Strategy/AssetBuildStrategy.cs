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

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EditorTools.AssetBundle
{
    public class StrategyNode
    {
        public string strategy; //节点所属策略名称
        public string processor;//节点资源分离处理器
        public string mode;     //节点资源打包模式
        public Regex pattern;   //节点资源分离正则表达式
    }

    public class PackageMode
    {
        /// <summary>
        /// 单一文件
        /// </summary>
        public const string SINGLE = "single";
        /// <summary>
        /// 选择文件集合
        /// </summary>
        public const string SELECTION = "selection";
        /// <summary>
        /// 文件夹
        /// </summary>
        public const string FOLDER = "folder";
    }
}

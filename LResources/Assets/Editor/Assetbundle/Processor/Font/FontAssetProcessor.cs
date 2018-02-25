using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle
{
    public class FontAssetProcessor : AssetProcessor
    {
        protected override Object GetAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(Font));
        }

        protected override HashSet<string> ApplyStrategyNode(string path, Object asset, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            string[] depentAssetPaths = AssetDatabase.GetDependencies(new string[] { path });
            foreach(string s in depentAssetPaths)
            {
                if(node.pattern.IsMatch(s))
                {
                    result.Add(s);
                }
            }
            return result;
        }
    }
}

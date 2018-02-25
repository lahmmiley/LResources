using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTools.AssetBundle
{
    /// <summary>
    /// 专供外部动态加载Icon图片的图集和Prefab打包策略节点
    /// </summary>
    public class AtlasProcessor : ComponentProcessor
    {
        public AtlasProcessor()
        {
            this.Name = "Atlas";
        }

        public override HashSet<string> Process(string entryPath, GameObject go, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            Image image = go.GetComponent<Image>();
            if(image != null)
            {
                string path = AssetDatabase.GetAssetPath(go);
                if(node.pattern.IsMatch(path))
                {
                    string folderPath = Path.GetDirectoryName(path);
                    image.spriteKey = AssetPathHelper.ReplaceSlash(AssetPathHelper.EliminaterStartToken(folderPath)) + "${0}";
                    result.Add(path);
                }
            }
            return result;
        }

    }
}

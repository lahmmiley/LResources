using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTools.AssetBundle
{
    public class TextFontProcessor : ComponentProcessor
    {
        public TextFontProcessor()
        {
            this.Name = "TextFont";
        }

        public override HashSet<string> Process(string entryPath, GameObject go, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            Text text = go.GetComponent<Text>();
            if(text != null && text.font != null)
            {
                string fontPath = AssetDatabase.GetAssetPath(text.font);
                if(node.pattern.IsMatch(fontPath))
                {
                    text.fontKey = AssetPathHelper.GetObjectKey(entryPath, fontPath, text.font, node);
                    text.font = null;
                    result.Add(fontPath);
                }
            }
            return result;
        }

    }
}

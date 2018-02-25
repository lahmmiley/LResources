using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTools.AssetBundle
{
    public class ImageShaderProcessor : ComponentProcessor
    {
        private MaterialShaderProcessor _shaderProcessor;

        public ImageShaderProcessor()
        {
            this.Name = "ImageShaderProcessor";
            _shaderProcessor = new MaterialShaderProcessor();
        }

        public override HashSet<string> Process(string entryPath, GameObject go, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            Image image = go.GetComponent<Image>();
            if(image != null && image.material != null)
            {
                result = _shaderProcessor.Process(entryPath, image.material, node);
            }
            return result;
        }
    }
}

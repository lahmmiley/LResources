using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTools.AssetBundle
{
    /// <summary>
    /// UI资源的材质及其依赖的图集资源使用依赖打包打在一起
    /// </summary>
    public class ImageMaterialProcessor : ComponentProcessor
    {
        public ImageMaterialProcessor()
        {
            this.Name = "ImageMaterial";
        }

        public override HashSet<string> Process(string entryPath, GameObject go, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            Image image = go.GetComponent<Image>();
            if(image != null && image.material != null)
            {
                string materialPath = MaterialAssetProcessor.GetMaterialPath(image.material);
                if(node.pattern.IsMatch(materialPath))
                {
                    result.Add(materialPath);
                    image.materialKey = AssetPathHelper.GetObjectKey(entryPath, materialPath, image.material, node);
                    image.material = null;
                    image.spriteKey = AssetPathHelper.GetObjectKey(entryPath, materialPath, image.sprite, node);
                    image.sprite = null;
                }
            }
            return result;
        }
    }
}

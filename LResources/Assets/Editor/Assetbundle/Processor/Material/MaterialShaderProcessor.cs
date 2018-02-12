using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle
{
    public class MaterialShaderProcessor
    {
        public MaterialShaderProcessor() { }

        public HashSet<string> Process(string entryPath, Material material, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            if(material == null)
            {
                Debug.LogError("资源打包出错， 缺少Material:" + entryPath);
            }
            string materialPath = MaterialAssetProcessor.GetMaterialPath(material);
            string shaderPath = MaterialAssetProcessor.GetShaderPath(material.shader);
            if(node.pattern.IsMatch(shaderPath))
            {
                MaterialJsonData jsonData = MaterialJsonData.GetMaterialJsonData(materialPath);
                jsonData.shaderFileName = GetShaderFileName(material.shader);
                jsonData.shaderKey = AssetPathHelper.GetObjectKey(entryPath, shaderPath, material.shader, node);
                jsonData.FillNoTexturePropertyData(material, node);
            }
        }

        private string GetShaderFileName(Shader shader)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}

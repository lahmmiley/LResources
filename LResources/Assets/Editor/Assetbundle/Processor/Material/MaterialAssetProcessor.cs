using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle
{
    public class MaterialAssetProcessor : AssetProcessor
    {
        public static string GetMaterialPath(Material material)
        {
            string path = AssetDatabase.GetAssetPath(material);
            if(Path.GetExtension(path) == string.Empty)
            {
                string msg = "使用了不带.mat后缀的内置Material，请使用项目创建的Material替代：" + material.name;
                AssetBundleExporter.ThrowException(msg);
            }
            return path;
        }

        public static string GetShaderPath(Shader shader)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            if(Path.GetExtension(path) == string.Empty)
            {
                string msg = "使用了不带.shader后缀的内置Shader，请使用项目创建的Shader：" + shader.name;
                AssetBundleExporter.ThrowException(msg);
            }
            return path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.UI
{
    public class MaterialCreator
    {
        public const string UI_DEFAULT_SHADER = "Assets/Resources/DefaultMaterials/UI-Default-ETC1.shader";

        public static void Create(string texturePath, string alphaTexturePath)
        {
            Shader shader = AssetDatabase.LoadAssetAtPath(UI_DEFAULT_SHADER, typeof(Shader)) as Shader;
            Material material = new Material(shader);
            Texture2D texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            Texture2D alphaTexture = AssetDatabase.LoadAssetAtPath(alphaTexturePath, typeof(Texture2D)) as Texture2D;
            material.SetTexture("_MainTex", texture);
            if(alphaTexture != null)
            {
                material.SetTexture("_AlphaTex", alphaTexture);
            }
            string materialPath = texturePath.Replace(".png", ".mat");
            AssetDatabase.CreateAsset(material, materialPath);
            AssetDatabase.SaveAssets();
        }
    }
}

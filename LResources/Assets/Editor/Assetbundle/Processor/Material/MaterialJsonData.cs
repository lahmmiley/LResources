using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle
{
    public class MaterialJsonData
    {
        public string type = "Material";
        public string shaderKey;
        public string shaderFileName;

        //Key为Material路径，Value为对应的MaterialJsonData
        private static Dictionary<string, MaterialJsonData> _materialJsonDataDict = new Dictionary<string, MaterialJsonData>();

        public static void Initialize()
        {
            _materialJsonDataDict.Clear();
        }

        public static MaterialJsonData GetMaterialJsonData(string materialPath)
        {
            if(!_materialJsonDataDict.ContainsKey(materialPath))
            {
                _materialJsonDataDict.Add(materialPath, new MaterialJsonData());
            }
            return _materialJsonDataDict[materialPath];
        }

        /// <summary>
        /// 将Shader和Material分离打包时(不包吃其依赖关系的分离)，Material不能独立存在
        /// 所以将Material中的属性以Json形式记录，并且最终序列化ScriptableObject的形式保存
        /// </summary>
        public List<List<string>> propertyTokenListList;

        private HashSet<string> _recordPropertySet;

        public MaterialJsonData()
        {
            propertyTokenListList = new List<List<string>>();
            _recordPropertySet = new HashSet<string>();
        }

        public void FillNoTexturePropertyData(Material material, StrategyNode node)
        {
            int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
            for(int i = 0; i < propertyCount; i++)
            {
                if(ShaderUtil.GetPropertyType(material.shader, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = ShaderUtil.GetPropertyName(material.shader, i);
                    if(_recordPropertySet.Contains(propertyName) == false)
                    {
                        _recordPropertySet.Add(propertyName);
                        List<string> list = GenerateNonTexturePropertyTokenList(material, i);
                        propertyTokenListList.Add(list);
                    }
                }
            }
        }

        private List<string> GenerateNonTexturePropertyTokenList(Material material, int propertyIndex)
        {
            List<string> result = new List<string>();
            ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(material.shader, propertyIndex);
            string propertyName = ShaderUtil.GetPropertyName(material.shader, propertyIndex);
            result.Add(propertyType.ToString());
            result.Add(propertyName);
            result.AddRange(FormatNonTextureProperty(material, propertyType, propertyName));
            return result;
        }

        private IEnumerable<string> FormatNonTextureProperty(Material material, ShaderUtil.ShaderPropertyType type, string propertyName)
        {
            List<string> result = new List<string>();
            switch(type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    result = FormatColor(material, propertyName);
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    result = FormatFloat(material, propertyName);
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    result = FormatFloat(material, propertyName);
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    result = FormatVector4(material, propertyName);
                    break;
            }
            return result;
        }

        private List<string> FormatColor(Material material, string propertyName)
        {
            Color color = material.GetColor(propertyName);
            return new List<string>() { color.r.ToString(), color.g.ToString(), color.b.ToString(), color.a.ToString() };
        }

        private List<string> FormatFloat(Material material, string propertyName)
        {
            return new List<string>() { material.GetFloat(propertyName).ToString() };
        }

        private List<string> FormatVector4(Material material, string propertyName)
        {
            Vector4 value = material.GetVector(propertyName);
            return new List<string>() { value.x.ToString(), value.y.ToString(), value.z.ToString(), value.w.ToString() };
        }

    }
}

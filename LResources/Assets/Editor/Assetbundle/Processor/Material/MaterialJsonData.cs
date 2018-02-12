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
                        List<string> list = 
                    }
                }
            }
        }
    }
}

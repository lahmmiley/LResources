﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle
{
    public class GameObjectAssetProcessor : AssetProcessor
    {
        private Dictionary<string, ComponentProcessor> _componentProcessorDict;

        public GameObjectAssetProcessor()
        {
            _componentProcessorDict = new Dictionary<string, ComponentProcessor>();

            AddComponentProcessor(new AnimatorControllerProcessor());
            AddComponentProcessor(new ImageMaterialProcessor());
            AddComponentProcessor(new ImageShaderProcessor());
            AddComponentProcessor(new TextFontProcessor());
        }

        private void AddComponentProcessor(ComponentProcessor processor)
        {
            _componentProcessorDict.Add(processor.Name, processor);
        }

        private ComponentProcessor GetComponentProcessor(string name)
        {
            if(!_componentProcessorDict.ContainsKey(name))
            {
                string msg = name + "Processor not found";
                AssetBundleExporter.ThrowException(msg);
            }
            return _componentProcessorDict[name];
        }

        protected override HashSet<string> ApplyStrategyNode(string entryPath, Object asset, StrategyNode node)
        {
            GameObject go = asset as GameObject;
            HashSet<string> result = new HashSet<string>();
            ComponentProcessor processor = GetComponentProcessor(node.processor);
            HashSet<string> sub = processor.Process(entryPath, go, node);
            result.UnionWith(sub);
            int count = go.transform.childCount;
            for(int i = 0; i < count; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                result.UnionWith(ApplyStrategyNode(entryPath, child, node));
            }
            return result;
        }

        /// <summary>
        /// 需要将处理后的Prefab保存起来
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="entryPath"></param>
        protected override void SaveAssets(Object asset, string path)
        {
            string tempPath = TemporaryAssetHelper.GetTempAssetPath(path);
            PrefabUtility.CreatePrefab(tempPath, asset as GameObject, ReplacePrefabOptions.ConnectToPrefab);
            Object.DestroyImmediate(asset, true);
        }

        protected override Object GetAsset(string path)
        {
            GameObject temp = TemporaryAssetHelper.CreateTempAsset(path) as GameObject;
            GameObject go = PrefabUtility.InstantiatePrefab(temp) as GameObject;
            return go;
        }
    }
}

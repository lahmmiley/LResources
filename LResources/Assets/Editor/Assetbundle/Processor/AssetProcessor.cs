using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle
{
    /// <summary>
    /// prefab meterial font animatorController等类型资源的处理器基类
    /// </summary>
    public class AssetProcessor
    {
        public const string ASSET_PREFAB = ".preafab";
        public const string ASSET_MATERIAL = ".mat";
        public const string ASSET_CONTROLLER = ".controller";
        public const string ASSET_FONT = ".ttf";

        private static AssetProcessor _assetProcessor;
        private static GameObjectAssetProcessor _gameObjectProcessor;
        private static MaterialAssetProcessor _materialProcessor;
        private static FontAssetProcessor _fontProcessor;

        static AssetProcessor()
        {
            _assetProcessor = new AssetProcessor();
            _gameObjectProcessor = new GameObjectAssetProcessor();
            _materialProcessor = new MaterialAssetProcessor();
            _fontProcessor = new FontAssetProcessor();
        }

        public static List<List<string>> ProcessAsset(string entryPath, AssetBuildStrategy strategy)
        {
            string extension = Path.GetExtension(entryPath.ToLower());
            switch(extension)
            {
                case ASSET_PREFAB:
                    return _gameObjectProcessor.Process(entryPath, strategy);
                case ASSET_MATERIAL:
                    return _materialProcessor.Process(entryPath, strategy);
                case ASSET_FONT:
                    return _fontProcessor.Process(entryPath, strategy);
            }
            return _assetProcessor.Process(entryPath, strategy);
        }

        /// <summary>
        /// 核心算法步骤：
        /// 1、获得资源的依赖列表List<string> dependAssetPathList = GetDependAssetPathList(entryPath);
        /// 2、根据节点正则表达式，获得该节点分离出来的资源列表pathSet = ApplyStrategyNode(entryPath, asset, node); pathList = pathSet.ToList()
        /// 3、移除pathList中和之前节点帅选结果相同的部分RemoveFilteredPath(filteredPathSet, pathList);
        /// 4、在dependAssetPathList中移除pathList，RemoveFromDependPathList(dependAssetPathList, pathList);
        /// 5、讲pathList加入筛选资源集合AddToFilteredPathSet(filteredPathSet, pathList);
        /// 6、若资源存在临时创建的副本，用副本替换原资源，对副本打包
        /// </summary>
        /// <param name="entryPath"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public virtual List<List<string>> Process(string entryPath, AssetBuildStrategy strategy)
        {
            Object asset = GetAsset(entryPath);
            List<string> dependAssetPathList = GetDependAssetPathList(entryPath);
            List<List<string>> pathListList = new List<List<string>>();
            HashSet<string> filteredPathSet = new HashSet<string>();
            for(int i = 0; i < strategy.nodeList.Count; i++)
            {
                StrategyNode node = strategy.nodeList[i];
                HashSet<string> pathSet;
                if(string.IsNullOrEmpty(node.processor) == false)
                {
                    pathSet = ApplyStrategyNode(entryPath, asset, node);
                }
                else
                {
                    pathSet = ApplyEmptyStrategyNode(dependAssetPathList, node);
                }
                List<string> pathList = pathSet.ToList();
                RemoveFilteredPath(filteredPathSet, pathList);
                RemoveFromDependPathList(dependAssetPathList, pathList);
                AddToFilteredPathSet(filteredPathSet, pathList);
                ReplaceWithTempAssetPath(pathList);
                pathListList.Add(pathList);
            }
            SaveAssets(asset, entryPath);
            return pathListList;
        }

        protected virtual void SaveAssets(Object asset, string entryPath){}

        private void ReplaceWithTempAssetPath(List<string> pathList)
        {
            for(int i = 0; i < pathList.Count; i++)
            {
                if(TemporaryAssetHelper.HasCreateTempAsset(pathList[i]))
                {
                    pathList[i] = TemporaryAssetHelper.GetTempAssetPath(pathList[i]);
                }
            }
        }

        private void AddToFilteredPathSet(HashSet<string> filteredPathSet, List<string> pathList)
        {
            foreach(string path in pathList)
            {
                filteredPathSet.Add(path);
            }
        }

        private void RemoveFromDependPathList(List<string> dependAssetPathList, List<string> pathList)
        {
            foreach(string path in pathList)
            {
                dependAssetPathList.Remove(path);
            }
        }

        private void RemoveFilteredPath(HashSet<string> filteredPathSet, List<string> pathList)
        {
            foreach(string path in filteredPathSet)
            {
                if(pathList.Contains(path))
                {
                    pathList.Remove(path);
                }
            }
        }

        /// <summary>
        /// 在资源上应用策略节点筛选资源
        /// </summary>
        /// <param name="entryPath"></param>
        /// <param name="asset"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual HashSet<string> ApplyStrategyNode(string entryPath, Object asset, StrategyNode node)
        {
            return new HashSet<string>();
        }

        private HashSet<string> ApplyEmptyStrategyNode(List<string> dependPathList, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            foreach(string path  in dependPathList)
            {
                if(node.pattern.IsMatch(path))
                {
                    result.Add(path);
                }
            }
            return result;
        }

        protected virtual Object GetAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        }

        public List<string> GetDependAssetPathList(string path)
        {
            List<string> pathList = AssetDatabase.GetDependencies(new string[] { path }).ToList<string>();
            return pathList;
        }

    }
}

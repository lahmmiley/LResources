using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using EditorTools.Assetbundle;
using Object = UnityEngine.Object;
using Logger = EditorTools.Assetbundle.Logger;
using System.Collections.Generic;
using System.IO;
using EditorTools.UI;
using System.Linq;

namespace EditorTools.AssetBundle
{
    public class AssetBundleExporter
    {
        internal const string LOGGER_NAME = "AssetBundle";
        //这些目录下的资源不进行打包
        internal static string[] IGNORE_PATHS = new string[] {"Assets/Things/data", "Assets/Things_temp"};

        private static HashSet<string> _processedAssetPathSet;

        [MenuItem("Assets/Build AssetBundle From Selection")]
        public static void BuildFromSelection()
        {
            AssetPathHelper.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            AssetPathHelper.patchVersion = DateTime.Now.ToString("yyyyMMddHHmm");
            Initialize();
            BuildAssets(GetSelectedAssetPathList());
        }

        private static void BuildAssets(List<string> selectedPathList)
        {
            //获取资源分离后的路径
            Dictionary<string, List<List<string>>> splitPathListListDict = GetSplitPathListListDict(selectedPathList);
            AssetDatabase.SaveAssets();
            BuildAssets(splitPathListListDict);
            AssetRecordHelper.WriteAssetRecord();
            AssetBundleBuilder.BuildAssetRecord();
            //TemporaryAssetHelper.DeleteAllTempAsset();
            if(AssetBuildStrategyManager.isSaveUIMediate == false)
            {
                UIPrefabProcessor.DeleteMediate();
            }
            AssetBundleBuilder.LogBuildResult();
        }

        /// <summary>
        /// 返回资源按策略节点分离后结果表
        /// key为入口资源路径
        /// Value为资源按策略节点分离后的列表的列表
        /// </summary>
        /// <param name="assetPathList"></param>
        /// <returns></returns>
        private static Dictionary<string, List<List<string>>> GetSplitPathListListDict(List<string> assetPathList)
        {
            Dictionary<string, List<List<string>>> result = new Dictionary<string, List<List<string>>>();
            string assetPath = null;
            foreach(string path in assetPathList)
            {
                if(IsBuildStrategyExists(path) == true
                    && result.ContainsKey(path) == false
                    && _processedAssetPathSet.Contains(path) == false)
                {
                    _processedAssetPathSet.Add(path);
                    assetPath = path;
                    if(path.StartsWith(UIPrefabProcessor.UI_PREFAB_ROOT) == true)
                    {
                        assetPath = PreprocessUIPrefab(path);
                    }
                    List<List<string>> pathListList = ProcessAsset(assetPath);
                    result.Add(path, pathListList);
                }
            }
            return result;
        }

        private static List<List<string>> ProcessAsset(string path)
        {
            AssetBuildStrategy strategy = AssetBuildStrategyManager.GetAssetBuildStrategy(path);
            if(strategy != null)
            {
                return AssetProcessor.ProcessAsset(path, strategy);
            }
            return new List<List<string>>();
        }

        /// <summary>
        /// 返回Key为资源Path,Value为该资源所在的BundlePath字典
        /// </summary>
        /// <param name="splitPathListListDict"></param>
        private static Dictionary<string, string> BuildAssets(Dictionary<string, List<List<string>>> splitPathListListDict)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach(string entryPath in splitPathListListDict.Keys)
            {
                List<List<string>> assetPathListList = splitPathListListDict[entryPath];
                List<StrategyNode> nodeList = AssetBuildStrategyManager.GetAssetBuildStrategy(entryPath).nodeList;
                HashSet<string> bundlePathSet = new HashSet<string>();
                for(int i = 0; i < assetPathListList.Count; i++)
                {
                    Dictionary<string, string> bundlePathDict = AssetBundleBuilder.Add(entryPath, assetPathListList[i], nodeList[i]);
                    foreach(string k in bundlePathDict.Keys)
                    {
                        string path = ReplaceTemparyPath(k);
                        if(result.ContainsKey(path) == false)
                        {
                            result.Add(path, bundlePathDict[k]);
                        }
                        bundlePathSet.Add(bundlePathDict[k]);
                    }
                }
                AssetRecordHelper.RecordAssetDependency(entryPath, bundlePathSet.ToList<string>());
            }
            AssetBundleBuilder.Build();
            return result;
        }

        /// <summary>
        /// 将记录中临时资源的路径替换为原始资源路径
        /// 包括:
        /// 1.打包过程中在Resources_temp下生成的临时文件
        /// 2.UI预处理过程中产生的临时文件
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private static string ReplaceTemparyPath(string tempPath)
        {
            ///处理含有Resource_temp的路径
            string result = tempPath.Replace(TemporaryAssetHelper.RESOURCES_TEMP, TemporaryAssetHelper.RESOURCES);
            //处理含有UI_{BuildTarget}的路径
            result = result.Replace(UIPrefabProcessor.GetShadowPrefabFolderRoot(), UIPrefabProcessor.UI_PREFAB_ROOT);
            return result;
        }

        /// <summary>
        /// 预处理UIPrefab，实际打包资源是使用图集资源的UIPrefab
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string PreprocessUIPrefab(string path)
        {
            return UIPrefabProcessor.Process(path);
        }

        private static bool IsBuildStrategyExists(string entryPath)
        {
            AssetBuildStrategy strategy = AssetBuildStrategyManager.GetAssetBuildStrategy(entryPath, false);
            if(strategy == null)
            {
                Logger.GetLogger(LOGGER_NAME).Log(string.Format("<color=#0000ff>未找到路径 {0} 对应的打包策略配置！</color>", entryPath));
                return false;
            }
            return true;
        }

        private static List<string> GetSelectedAssetPathList()
        {
            Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            List<string> result = new List<string>();
            foreach(Object obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if(VerifyAssetPath(path) == true)
                {
                    result.Add(path);
                }
            }
            return result;
        }

        private static bool VerifyAssetPath(string path)
        {
            if(Path.GetExtension(path) == string.Empty)
            {
                return false;
            }
            foreach (string s in IGNORE_PATHS)
            {
                if(path.Contains(s) == true)
                {
                    Logger.GetLogger(LOGGER_NAME).Log(string.Format("<color=#0000ff>路径 {0} 资源设置为不需要打包！</color>", path));
                    return false;
                }
            }
            return true;
        }

        private static void Initialize()
        {
            TemporaryAssetHelper.Initialize();
            MaterialJsonData.Initialize();
            AssetBuildStrategyManager.Initialize();
            AssetBundleBuilder.Initialize();
            AssetRecordHelper.ReadAssetRecord();
            Logger.GetLogger(LOGGER_NAME).Level = Logger.LEVEL_LOG;
            _processedAssetPathSet = new HashSet<string>();
        }

        public static void ThrowException(string msg)
        {
            EditorUtility.DisplayDialog("错误", msg, "马上调整Go~");
            throw new Exception(msg);
        }
    }
}



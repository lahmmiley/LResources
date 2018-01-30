using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using EditorTools.Assetbundle;
using Object = UnityEngine.Object;
using Logger = EditorTools.Assetbundle.Logger;
using System.Collections.Generic;
using System.IO;

namespace EditorTools.AssetBundle
{
    public class AssetBundleExporter
    {
        internal const string LOGGER_NAME = "AssetBundle";
        //这些目录下的资源不进行打包
        internal static string[] IGNORE_PATHS = new string[] {"Assets/Things/data", "Assets/Things_temp"};

        private static HashSet<string> _processedAssetPathSet;

        public static void BuildFromSelection()
        {
            AssetPathHelper.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            AssetPathHelper.patchVersion = DateTime.Now.ToString("yyyyMMddHHmm");
            Initialize();
            BuildAssets(GetSelectedAssetPathList());
        }

        private static void BuildAssets(List<string> selectedPathList)
        {
            Dictionary<string, List<List<string>>> splitPathListListDict = GetSplitPathListListDict(selectedPathList);
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
                    //if(path.StartsWith(UIPr))
                }
            }
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
            //TemporaryAssetHelper.Initialize();
            //MeterialJsonData.Initialize();
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



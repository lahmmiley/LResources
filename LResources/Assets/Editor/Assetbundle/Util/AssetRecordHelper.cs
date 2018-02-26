using Game.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle
{
    public class AssetRecordHelper
    {
        public const string ASSET_RECORD_NAME = "_resources.asset";
        public const string ASSET_RECORD_PATH = "Assets/Things/_resources.asset";

        private static bool _isRecordDirty = false;

        private static Dictionary<string, List<string>> _dependentPhysicalPathListDict;

        public static void ReadAssetRecord()
        {
            _dependentPhysicalPathListDict = new Dictionary<string, List<string>>();
            AssetRecordScriptableObject obj = AssetDatabase.LoadAssetAtPath(ASSET_RECORD_PATH, typeof(AssetRecordScriptableObject)) as AssetRecordScriptableObject;
            if (obj == null) return;

            try
            {
                for(int i = 0; i < obj.dependencyEntries.Length; i++)
                {
                    AssetDependencyEntry entry = obj.dependencyEntries[i];
                    _dependentPhysicalPathListDict.Add(entry.path, entry.physicalPaths.ToList());
                }
            } catch(Exception e)
            {
                Debug.LogError("_resources.asset 文件格式错误！");
                Debug.LogError(e.Message);
            }
        }

        public static void RecordAssetDependency(string entryPath, List<string> bundlePathList)
        {
            string key = AssetPathHelper.EliminaterStartToken(entryPath);
            if(_dependentPhysicalPathListDict.ContainsKey(key) == false)
            {
                _dependentPhysicalPathListDict.Add(key, bundlePathList);
                _isRecordDirty = true;
                return;
            }

            List<string> existBundlePathList = _dependentPhysicalPathListDict[key];

            if(existBundlePathList.Count != bundlePathList.Count)
            {
                _dependentPhysicalPathListDict[key] = bundlePathList;
                _isRecordDirty = true;
                return;
            }
            for(int i = 0; i < existBundlePathList.Count; i++)
            {
                if(existBundlePathList[i] != bundlePathList[i])
                {
                    _dependentPhysicalPathListDict[key] = bundlePathList;
                    _isRecordDirty = true;
                    return;
                }
            }
        }

        public static void WriteAssetRecord()
        {
            if(_isRecordDirty == true)
            {
                AssetRecordScriptableObject obj = CreateScriptableObject();
                AssetDatabase.CreateAsset(obj, ASSET_RECORD_PATH);
            }
        }

        private static AssetRecordScriptableObject CreateScriptableObject()
        {
            string[] keys = CollectionSortHelper.GetSortedArray(_dependentPhysicalPathListDict.Keys);
            AssetRecordScriptableObject obj = ScriptableObject.CreateInstance<AssetRecordScriptableObject>();
            obj.dependencyEntries = new AssetDependencyEntry[keys.Length];
            for(int i = 0; i < keys.Length; i++)
            {
                AssetDependencyEntry entry = new AssetDependencyEntry();
                entry.path = keys[i];
                List<string> dependentPhysicalPathList = _dependentPhysicalPathListDict[keys[i]];
                entry.physicalPaths = new string[dependentPhysicalPathList.Count];
                for(int j = 0; j < dependentPhysicalPathList.Count; j++)
                {
                    entry.physicalPaths[j] = dependentPhysicalPathList[j];
                }
                obj.dependencyEntries[i] = entry;
            }
            return obj;
        }
    }
}

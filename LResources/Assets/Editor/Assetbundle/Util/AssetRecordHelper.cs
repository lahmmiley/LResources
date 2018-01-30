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

    }
}

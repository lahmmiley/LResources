using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle
{
    public class TemporaryAssetHelper
    {
        public const string RESOURCES = "Assets/Things";
        public const string RESOURCES_TEMP = "Assets/Things_temp";

        /// <summary>
        /// 临时资源表，Key为临时资源路径
        /// </summary>
        private static Dictionary<string, Object> _tempAssetDict;
        private static HashSet<string> _assetPathSet;
        private static HashSet<string> _tempAssetPathSet;

        public static void Initialize()
        {
            _tempAssetDict = new Dictionary<string, Object>();
            _assetPathSet = new HashSet<string>();
            _tempAssetPathSet = new HashSet<string>();
        }

        public static bool HasCreateTempAsset(string assetPath)
        {
            return _assetPathSet.Contains(assetPath);
        }

        public static string GetTempAssetPath(string path)
        {
            string tempAssetPath = path.Replace(RESOURCES, RESOURCES_TEMP);
            string folderPath = Path.GetDirectoryName(AssetPathHelper.ToFileSystemPath(tempAssetPath));
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return tempAssetPath;
        }
    }
}

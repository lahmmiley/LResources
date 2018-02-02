using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorTools.UI
{
    public class AtlasWriter
    {
        public const int ATLAS_MAX_SIZE = 2048;
        public const int FAVOR_MAX_SIZE = 2048;

        public static void Write(Texture2D atlas, string path)
        {
            byte[] pngData = atlas.EncodeToPNG();
            string pngPath = Application.dataPath + path.Replace("Assets", "");
            File.WriteAllBytes(pngPath, pngData);

            LogAtlasSize(atlas, path);
        }

        private static void LogAtlasSize(Texture2D atlas, string path)
        {
            if(atlas.width > FAVOR_MAX_SIZE || atlas.height > FAVOR_MAX_SIZE)
            {
                Debug.Log(string.Format("<color=#ff0000>【警告】图集宽度或高度超过1024像素： {0} </color>", path));
            }
            else
            {
                Debug.Log(string.Format("<color=#0000ff>图集 {0} 尺寸为： {1}x{2}</color>", path, atlas.width, atlas.height));
            }
        }
    }
}

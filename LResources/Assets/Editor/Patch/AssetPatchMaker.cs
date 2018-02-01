using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EditorTools.Patch
{
    public class AssetPatchMaker
    {
        public static string GetFileLastWriteTime(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.LastWriteTime.ToString("yyyyMMddHHmmss");
        }
    }
}

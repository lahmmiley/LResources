using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorTools.UI
{
    public class ImageChannelSpliterWrapper
    {
        public static void Execute(string pngPath)
        {
            pngPath = string.Concat(Application.dataPath.Replace("/Assets", "/"), pngPath);
            string toolPath = string.Concat(Application.dataPath.Replace("/Assets", "/"), "Tool");
            string alphaPath = pngPath.Replace(".png", "_alpha.png");
            Process process = new Process();
            string paramContent = string.Format("\"{0}\" \"{1}\" \"{2}\"", pngPath, alphaPath, toolPath);
            string batPath = toolPath + "/ImageChannelSpliter.bat ";
            ProcessStartInfo info = new ProcessStartInfo(batPath, paramContent);
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorTools.Assetbundle
{
    public class Logger
    {
        public const int LEVEL_LOG = 0;
        public const int LEVEL_WARNING = 1;
        public const int LEVEL_ERROR = 2;
        public const int LEVEL_EXCEPTION = 3;
        private static Dictionary<string, Logger> _loggerDict = new Dictionary<string, Logger>();

        public int Level { get; set; }

        private Logger()
        {
        }

        public static Logger GetLogger(string name)
        {
            if(!_loggerDict.ContainsKey(name))
            {
                _loggerDict.Add(name, new Logger());
            }
            return _loggerDict[name];
        }

        public void Log(string content)
        {
            if(Level <= LEVEL_LOG)
            {
                Debug.Log(content);
            }
        }

        public void Warning(string content)
        {
            if(Level <= LEVEL_WARNING)
            {
                Debug.LogWarning(content);
            }
        }

        public void Error(string content)
        {
            if(Level <= LEVEL_ERROR)
            {
                Debug.LogWarning(content);
            }
        }

        public void Exception(Exception e)
        {
            if(Level <= LEVEL_EXCEPTION)
            {
                Debug.LogException(e);
            }
        }
    }
}

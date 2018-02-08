using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.AssetBundle
{
    public abstract class ComponentProcessor
    {
        public string Name { get; set; }
        public abstract HashSet<string> Process(string entryPath, GameObject go, StrategyNode node);
    }
}

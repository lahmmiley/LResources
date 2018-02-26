using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorTools.AssetBundle
{
    /// <summary>
    /// 若业务需要可以将AnimatorControllerProcessor和AnimationClip进行分离打包
    /// </summary>
    public class AnimatorControllerProcessor : ComponentProcessor
    {
        public AnimatorControllerProcessor()
        {
            this.Name = "AnimatorControllerProcessor";
        }

        //TODO
        public override HashSet<string> Process(string entryPath, UnityEngine.GameObject go, StrategyNode node)
        {
            throw new NotImplementedException();
        }
    }
}

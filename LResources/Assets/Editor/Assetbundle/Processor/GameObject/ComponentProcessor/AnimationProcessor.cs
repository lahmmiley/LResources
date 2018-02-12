using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AssetBundle
{
    public class AnimationProcessor : ComponentProcessor
    {
        public AnimationProcessor()
        {
            this.Name = "Animation";
        }

        // TODO 看不懂
        public override HashSet<string> Process(string entryPath, GameObject go, StrategyNode node)
        {
            HashSet<string> result = new HashSet<string>();
            Animation animation = go.GetComponent<Animation>();
            if(animation != null)
            {
                AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);
                AnimationClip defaultClip = animation.clip;
                VerifyClipsName(entryPath, clips);
                string[] clipTokens = GetClipTokens(go, clips.Length);
                if(defaultClip != null)
                {
                    string defaultClipPath = AssetDatabase.GetAssetPath(defaultClip);
                    if(node.pattern.IsMatch(defaultClipPath))
                    {
                        string defaultClipKey = AssetPathHelper.GetObjectKey(entryPath, defaultClipPath, defaultClip, node);
                        result.Add(defaultClipPath);
                        animation.clip = null;
                        clipTokens[0] = defaultClipKey;
                    }
                }

                for(int i = 0; i < clips.Length; i++)
                {
                    AnimationClip clip = clips[i];
                    if(clip != null)
                    {
                        string clipPath = AssetDatabase.GetAssetPath(clip);
                        if(node.pattern.IsMatch(clipPath))
                        {
                            string clipKey = AssetPathHelper.GetObjectKey(entryPath, clipPath, clip, node);
                            result.Add(clipPath);
                            animation.RemoveClip(clip);
                            clipTokens[i + 1] = clipKey;
                        }
                    }
                }
                if(IsClipTokensEmpty(clipTokens) == false)
                {
                    AssetBridgeHelper.AddEntry(go, this.Name, clipTokens);
                }
            }
            return result;
        }

        //当ClipTokens都为空的时候，表示没有分离出AnimationClip出来进行单独打包，也就不需要在Go上添加AssetBridge
        private bool IsClipTokensEmpty(string[] clipTokens)
        {
            for (int i = 0; i < clipTokens.Length; i++)
            {
                if (string.IsNullOrEmpty(clipTokens[i]) == false)
                {
                    return false;
                }
            }
            return true;
        }

        //clipTokens记录AnimationClip的Key，其中第一个值默认clip的key值
        private string[] GetClipTokens(GameObject go, int clipCount)
        {
            string[] clipTokens = AssetBridgeHelper.GetEntryTokens(go, this.Name);
            if(clipTokens == null)
            {
                clipTokens = new string[clipCount + 1];
                for(int i = 0; i < clipTokens.Length; i++)
                {
                    clipTokens[i] = string.Empty;
                }
            }
            return clipTokens;
        }

        private void VerifyClipsName(string entryPath, AnimationClip[] clips)
        {
            HashSet<string> hashSet = new HashSet<string>();
            for(int i = 0; i < clips.Length; i++)
            {
                if(clips[i] != null)
                {
                    if(hashSet.Contains(clips[i].name))
                    {
                        string msg = entryPath + " Animation中存在同名AnimationClip,需要对相关AniamtionClip重命名！";
                        AssetBundleExporter.ThrowException(msg);
                    }
                }
                hashSet.Add(clips[i].name);
            }
        }
    }
}

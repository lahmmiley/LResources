using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Asset
{
    public class AssetBridge : MonoBehaviour
    {
        [SerializeField]
        public AssetEntry[] entries = new AssetEntry[0];

        protected void Awake()
        {
            if(Parse != null)
            {
                Parse(gameObject, entries);
            }
        }

        public static Action<GameObject, AssetEntry[]> Parse;

        public static void AddEntry(AssetBridge bridge, AssetEntry entry)
        {
            Array.Resize<AssetEntry>(ref bridge.entries, bridge.entries.Length + 1);
            bridge.entries[bridge.entries.Length - 1] = entry;
        }
    }

    [Serializable]
    public class AssetEntry
    {
        [SerializeField]
        public string asset;
        [SerializeField]
        public string[] tokens = new string[0];

        public static void AddToken(AssetEntry entry, string token)
        {
            Array.Resize<string>(ref entry.tokens, entry.tokens.Length + 1);
            entry.tokens[entry.tokens.Length - 1] = token;
        }
    }
}

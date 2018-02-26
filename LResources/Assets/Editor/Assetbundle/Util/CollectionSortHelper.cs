using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorTools.AssetBundle
{
    public class CollectionSortHelper
    {
        public static string[] GetSortedArray(ICollection collection)
        {
            string[] result = new string[collection.Count];
            collection.CopyTo(result, 0);
            Array.Sort<string>(result);
            return result;
        }
    }
}

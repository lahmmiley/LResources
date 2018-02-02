using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorTools.UI
{
    public class Scale9GridTextureProcessor
    {
        public static Texture2D Process(Texture2D source, int top, int right, int bottom, int left)
        {
            int sourceWidth = source.width;
            int sourceHeight = source.height;
            Color32[] sourcePixels = source.GetPixels32();
            int targetWidth = left + 1 + right;
            int targetHeight = top + 1 + bottom;
            Color32[] targetPixels = new Color32[targetWidth * targetHeight];
            Texture2D target = new Texture2D(targetWidth, targetHeight);
            int pixelIndex = 0;
            for(int i = 0; i < sourceHeight; i++)
            {
                if(i > bottom && i < (sourceHeight - top))
                {
                    continue;
                }
                for(int j = 0; j < sourceWidth; j++)
                {
                    if(j > left && j < (sourceWidth - right))
                    {
                        continue;
                    }
                    targetPixels[pixelIndex++] = sourcePixels[i * sourceWidth + j];
                }
            }
            target.SetPixels32(targetPixels);
            target.Apply();
            return target;
        }
    }
}

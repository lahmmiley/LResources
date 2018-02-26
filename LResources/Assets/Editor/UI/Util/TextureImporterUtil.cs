using EditorTools.AssetBundle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.UI
{
    public class TextureImporterUtil
    {
        public static void CreateAlphaChannelImporter(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Default;
            importer.isReadable = false;
            importer.mipmapEnabled = false;
            importer.textureFormat = GetTextureFormat();
        }

        public static void CreateMultipleSpriteImporter(string path, Rect[] rects, string[] spriteNames, Vector4[] borders, int width, int height, TextureImporterFormat format, int maxSize)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 1;
            SpriteMetaData[] metaDatas = new SpriteMetaData[spriteNames.Length];
            for(int i = 0; i < metaDatas.Length; i++)
            {
                SpriteMetaData metaData = new SpriteMetaData();
                metaData.name = spriteNames[i];
                Rect rect = rects[i];
                if(rects.Length > 1)
                {
                    metaData.rect = new Rect(rect.xMin * width + TextureClamper.BORDER, rect.yMin * height + TextureClamper.BORDER, rect.width * width - TextureClamper.BORDER * 2, rect.height * height - TextureClamper.BORDER * 2);
                }
                else
                {
                    metaData.rect = new Rect(rect.xMin * width, rect.yMin * height, rect.width * width, rect.height * height);
                }
                if(borders != null)
                {
                    metaData.border = borders[i];
                }
                metaData.pivot = new Vector2(0.5f, 0.5f);
                metaDatas[i] = metaData;
            }
            importer.spritesheet = metaDatas;
            importer.maxTextureSize = maxSize;
            importer.isReadable = false;
            importer.mipmapEnabled = false;
            importer.textureFormat = format;
        }

        public static TextureImporterFormat GetTextureFormat()
        {
            switch(AssetPathHelper.GetBuildTarget())
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return TextureImporterFormat.DXT5;
                case BuildTarget.Android:
                    return TextureImporterFormat.ETC_RGB4;
                case BuildTarget.iOS:
                    return TextureImporterFormat.PVRTC_RGBA4;
            }
            return TextureImporterFormat.DXT1;
        }
    }
}

using EditorTools.AssetBundle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

//完全一致
namespace EditorTools.UI
{
    public class AtlasGenerator
    {
        public const int ATLAS_MAX_SIZE = 2048;

        public static string[] Generate(string[] folderPaths)
        {
            string[] result = new string[folderPaths.Length];
            for(int i = 0; i < folderPaths.Length; i++)
            {
                string folderPath = folderPaths[i];
                string[] assetPaths = GetAssetPaths(folderPath);
                string tag = GetFolderTexturePackingTag(assetPaths);
                string atlasPath = GetAtlasPath(folderPath, tag);
                //当文件列表的md5变化了才重新生成图集
                bool isMd5Changed = FolderFilesMD5Checker.IsFilesChanged(folderPath);
                if(isMd5Changed)
                {
                    ImportTextureReadable(assetPaths);
                    TextureData[] textureDatas = ReadTexture(assetPaths);
                    CreateAtlas(atlasPath, textureDatas);
                }
                else
                {
                    Debug.Log("文件列表Md5值未变化，不需要重新生成图集~~ " + atlasPath);
                }
                result[i] = atlasPath;
            }
            return result;
        }

        public static string GetAtlasPath(string folderPath, string tag)
        {
            string atlasPath = UIPrefabProcessor.GetShadowTextureFolderPath(folderPath) + "/UI_" + tag + ".png";
            UIPrefabProcessor.CreateInexistentFolder(atlasPath);
            return atlasPath;
        }

        public static string GetMaterialPath(string atlasPath)
        {
            return atlasPath.Replace(".png", ".mat");
        }

        private static string[] GetAssetPaths(string folderPath)
        {
            string systemPath = UIPrefabProcessor.ToFileSystemPath(folderPath);
            string[] filePaths = Directory.GetFiles(systemPath, "*.png");
            string[] result = new string[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                result[i] = UIPrefabProcessor.ToAssetPath(filePaths[i]);
            }
            return result;
        }

        /// <summary>
        /// 获取目录下Texture的Packingtag
        /// 同时验证同一目录下Texture的PackingTag是否相同，不同则报错，提示修改
        /// </summary>
        /// <param name="assetPaths"></param>
        /// <returns></returns>
        private static string GetFolderTexturePackingTag(string[] assetPaths)
        {
            string tag = null;
            foreach (string s in assetPaths)
            {
                string nextTag = GetTexturePackingTag(s);
                if (tag != null && tag != nextTag)
                {
                    UIPrefabProcessor.ThrowException("同一目录下存在不同的SpritePackingTag，路径： " + s + " tag: " + nextTag);
                }
                else
                {
                    tag = nextTag;
                }
            }
            return tag;
        }

        private static string GetTexturePackingTag(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            return importer.spritePackingTag;
        }

        private static void ImportTextureReadable(string[] assetPaths)
        {
            foreach (string s in assetPaths)
            {
                TextureImporter importer = AssetImporter.GetAtPath(s) as TextureImporter;
                importer.isReadable = true;
                AssetDatabase.ImportAsset(s, ImportAssetOptions.ForceUpdate);
            }
        }

        private static TextureData[] ReadTexture(string[] assetPaths)
        {
            TextureData[] textureDatas = new TextureData[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPaths[i], typeof(Sprite)) as Sprite;
                TextureData data = new TextureData();
                data.name = sprite.name;
                Vector4 border = sprite.border;
                data.top = (int)border.w;
                data.right = (int)border.z;
                data.bottom = (int)border.y;
                data.left = (int)border.x;
                Texture2D texture = AssetDatabase.LoadAssetAtPath(assetPaths[i], typeof(Texture2D)) as Texture2D;
                /*
                if(data.IsScale9Grid)
                {
                    texture = Scale9GridTextureProcessor.Process(texture, data.top, data.right, data.bottom, data.left);
                }
                */
                if (textureDatas.Length > 1)
                {
                    texture = TextureClamper.Clamp(texture);
                }
                else
                {
                    texture = TextureClamper.ClampSingle(texture);
                }
                data.texture = texture;
                textureDatas[i] = data;
            }
            return textureDatas;
        }

        private static Texture2D CreateAtlas(string atlasPath, TextureData[] textureDatas)
        {
            Texture2D atlas = new Texture2D(ATLAS_MAX_SIZE, ATLAS_MAX_SIZE);
            Rect[] rects = atlas.PackTextures(GetPackTextures(textureDatas), 0, ATLAS_MAX_SIZE, false);
            AtlasWriter.Write(atlas, atlasPath);
            int atlasWidth = atlas.width;
            int atlasHeight = atlas.height;
            string alphaAtlasPath = atlasPath.Replace(".png", "_alpha.png");
            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);
            //安卓平台图集分离通道
            if(AssetPathHelper.GetBuildTarget() == BuildTarget.Android && !atlasPath.Contains("UI_Base"))
            {
                try
                {
                    ImageChannelSpliterWrapper.Execute(atlasPath);
                    AssetDatabase.ImportAsset(alphaAtlasPath, ImportAssetOptions.ForceUpdate);
                    TextureImporterUtil.CreateAlphaChannelImporter(alphaAtlasPath);
                    AssetDatabase.ImportAsset(alphaAtlasPath, ImportAssetOptions.ForceUpdate);
                }
                catch (Exception e)
                {
                    Debug.LogError("通道分离过程中发生错误： " + e.Message);
                    Debug.LogException(e);
                }
            }
            TextureImporterFormat textureFormat = TextureImporterUtil.GetTextureFormat();
            if(atlasPath.Contains("UI_Base"))
            {
                textureFormat = TextureImporterFormat.ARGB32;
            }
            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);
            TextureImporterUtil.CreateMultipleSpriteImporter(atlasPath, rects, GetPackTexturesNames(textureDatas), GetSpriteBorders(textureDatas),
                                                                atlasWidth, atlasHeight, textureFormat, ATLAS_MAX_SIZE);
            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);
            MaterialCreator.Create(atlasPath, alphaAtlasPath);
            return atlas;
        }

        private static Texture2D[] GetPackTextures(TextureData[] textureDatas)
        {
            Texture2D[] result = new Texture2D[textureDatas.Length];
            for (int i = 0; i < textureDatas.Length; i++)
            {
                result[i] = textureDatas[i].texture;
            }
            return result;
        }

        private static string[] GetPackTexturesNames(TextureData[] textureDatas)
        {
            string[] result = new string[textureDatas.Length];
            for (int i = 0; i < textureDatas.Length; i++)
            {
                result[i] = textureDatas[i].name;
            }
            return result;
        }

        private static Vector4[] GetSpriteBorders(TextureData[] textureDatas)
        {
            Vector4[] result = new Vector4[textureDatas.Length];
            for (int i = 0; i < textureDatas.Length; i++)
            {
                TextureData textureData = textureDatas[i];
                result[i] = new Vector4(textureData.left, textureData.bottom, textureData.right, textureData.top);
            }
            return result;
        }
    }

    public class TextureData
    {
        public string name;
        public Texture2D texture;
        public int width;
        public int height;
        public int top;
        public int right;
        public int bottom;
        public int left;

        //剔除低透明区域后图片相对于原图片边缘的Padding值
        public Vector4 padding = Vector4.zero;

        public bool IsScale9Grid
        {
            get
            {
                return top > 0 && right > 0 && bottom > 0 && left > 0;
            }
        }
    }
}

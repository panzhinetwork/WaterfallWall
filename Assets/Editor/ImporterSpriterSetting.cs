using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImporterSpriterSetting : EditorWindow
{
    [MenuItem("Importer/ImporterSpriterSetting")]
    static void ImporterSpriterSettingWindow()
    {
        //创建窗口
        Rect wr = new Rect(500, 500, 500, 500);
        ImporterSpriterSetting window = (ImporterSpriterSetting)EditorWindow.GetWindowWithRect(typeof(ImporterSpriterSetting), wr, false, "ImporterSpriterSetting");
        window.Show();
    }

    TextureImporterType textureType = TextureImporterType.Sprite;
    SpriteImportMode spriteMode = SpriteImportMode.Single;
    string packingTag = string.Empty;
    bool sRGBTexture = true;
    bool alphaIsTransparency = true;
    bool mipmapEnabled = false;
    TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    FilterMode filterMode = FilterMode.Bilinear;
    int maxTextureSize = 2048;
    string[] names = { "16", "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };
    int[] sizes = { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
    //TextureImporterFormat textureImporterFormatAndroid = TextureImporterFormat.ETC2_RGBA8;
    //TextureImporterFormat textureImporterFormatiPhone = TextureImporterFormat.PVRTC_RGBA4;
    //TextureImporterFormat textureImporterFormatStandalone = TextureImporterFormat.DXT5;

    void OnGUI()
    {

        textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", textureType);
        EditorGUILayout.Space();//空一行

        maxTextureSize = EditorGUILayout.IntPopup("Max Texture Size", maxTextureSize, names, sizes);
        EditorGUILayout.Space();//空一行

        //spriteMode = (SpriteImportMode)EditorGUILayout.EnumPopup("Sprite Mode", spriteMode);
        //EditorGUILayout.Space();//空一行

        //packingTag = EditorGUILayout.TextField("Packing Tag", packingTag);
        //EditorGUILayout.Space();//空一行

        //sRGBTexture = EditorGUILayout.Toggle("S RGB Texture", sRGBTexture);
        //EditorGUILayout.Space();//空一行

        //alphaIsTransparency = EditorGUILayout.Toggle("Alpha Is Transparency", alphaIsTransparency);
        //EditorGUILayout.Space();//空一行

        //mipmapEnabled = EditorGUILayout.Toggle("Generate Mip Map", mipmapEnabled);
        //EditorGUILayout.Space();//空一行

        wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", wrapMode);
        EditorGUILayout.Space();//空一行

        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", filterMode);
        EditorGUILayout.Space();//空一行

        //textureImporterFormatAndroid = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Importer Format For Android", textureImporterFormatAndroid);
        //EditorGUILayout.Space();//空一行

        //textureImporterFormatiPhone = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Importer Format For iPhone", textureImporterFormatiPhone);
        //EditorGUILayout.Space();//空一行

        //textureImporterFormatStandalone = (TextureImporterFormat)EditorGUILayout.EnumPopup("Texture Importer Format For Standalone", textureImporterFormatStandalone);
        //EditorGUILayout.Space();//空一行



        if (GUILayout.Button("设置Sprite", GUILayout.ExpandWidth(true)))
        {
            Object[] targetObj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);//得到选中的文件，包括选中文件夹的子文件和子文件夹
            if (targetObj != null && targetObj.Length > 0)
            {
                for (int i = 0; i < targetObj.Length; i++)
                {
                    if (targetObj[i] is Texture)                                  //检测是不是Texture
                    {
                        string path = AssetDatabase.GetAssetPath(targetObj[i]);   //得到资源的路径
                        TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter; //通过路径得到资源
                        if (texture.textureType == textureType && texture.maxTextureSize == maxTextureSize)
                            continue;

                        texture.textureType = textureType;                        //TextureType (Enum)
                        texture.spriteImportMode = spriteMode;                    //SpriteMode  (Enum)
                        texture.spritePackingTag = packingTag;                    //packingTag  (String)
                        texture.spritePixelsPerUnit = 100;                        //PixelsPerUnit （flot ps：像素）
                        texture.sRGBTexture = sRGBTexture;                        //sRGBTexture  （bool）
                        texture.alphaIsTransparency = alphaIsTransparency;        //AlphaIsTransparency （bool）
                        texture.filterMode = filterMode;                          //FilterMode     （Enum）
                        texture.mipmapEnabled = mipmapEnabled;                    //mipmapEnabled  （Bool）
                        texture.wrapMode = wrapMode;                              //WrapMode  (Enum)   
                        texture.maxTextureSize = maxTextureSize;
                        texture.textureCompression = TextureImporterCompression.Uncompressed;

                        TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings();//用来单独设置单独的平台信息
                        platformSetting.name = "Default";                         //平台名字
                        platformSetting.format = TextureImporterFormat.Automatic; //Format属性
                        platformSetting.overridden = true;                        //True为勾选Override选项
                        //platformSetting.maxTextureSize = maxTextureSize;          //最大纹理尺寸
                        texture.SetPlatformTextureSettings(platformSetting);      //将平台信息设置给图片

                        //TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings();//用来单独设置单独的平台信息
                        //platformSetting.name = "Android";                         //平台名字
                        //platformSetting.format = textureImporterFormatAndroid;    //Format属性
                        //platformSetting.overridden = true;                        //True为勾选Override选项
                        //platformSetting.maxTextureSize = 2048;                    //最大纹理尺寸
                        //texture.SetPlatformTextureSettings(platformSetting);      //将平台信息设置给图片

                        //TextureImporterPlatformSettings platformSettingIP = new TextureImporterPlatformSettings();
                        //platformSettingIP.name = "iPhone";
                        //platformSettingIP.format = textureImporterFormatiPhone;
                        //platformSettingIP.overridden = true;
                        //platformSettingIP.maxTextureSize = 2048;
                        //texture.SetPlatformTextureSettings(platformSettingIP);

                        //TextureImporterPlatformSettings platformSettingStandalone = new TextureImporterPlatformSettings();
                        //platformSettingStandalone.name = "Standalone";
                        //platformSettingStandalone.format = textureImporterFormatStandalone;
                        //platformSettingStandalone.overridden = true;
                        //platformSettingStandalone.maxTextureSize = 2048;
                        //texture.SetPlatformTextureSettings(platformSettingStandalone);

                        AssetDatabase.ImportAsset(path);
                    }
                }
            }
        }
    }
}
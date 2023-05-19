using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class UIEditorTools
{
    [MenuItem("Tools/UI Editors/SetUIDirectoryUseSpriteFormat")]
    private static void SetUIDirectoryUseSpriteFormat()
    {
        string path = Application.dataPath + "/Art/UI";
        List<string> files = GetAllFiles(path);

        foreach (var file in files)
        {
            if (!file.EndsWith(".png") && !file.EndsWith(".jpg"))
            {
                continue;
            }

            string assetPath = file.Replace(Application.dataPath, "Assets");
            TextureImporter texture = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            texture.textureType = TextureImporterType.Sprite;
            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.textureCompression = TextureImporterCompression.Uncompressed;
            texture.SetPlatformTextureSettings(settings);
            AssetDatabase.ImportAsset(assetPath);
        }

        AssetDatabase.Refresh();
        Debug.Log("ui 设置成功");
    }

    private static List<string> GetAllFiles(string path)
    {
        List<string> fileList = new List<string>();
        string[] files = Directory.GetFiles(path);
        if (files.Length > 0)
        {
            fileList.AddRange(files);
        }

        string[] dirs = Directory.GetDirectories(path);
        foreach (var dir in dirs)
        {
           List<string> childFileList =  GetAllFiles(dir);
            if (childFileList.Count > 0)
            {
                fileList.AddRange(childFileList);
            }
        }

        return fileList;
    }
}

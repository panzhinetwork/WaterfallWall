using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

[CustomEditor(typeof(SpriteAnimation))]
public class SpriteAnimationInspector : Editor
{
    private SpriteAnimation _spriteAnimation;
    private bool _reverseoOrder;

    void OnEnable()
    {
        _spriteAnimation = target as SpriteAnimation;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _reverseoOrder = GUILayout.Toggle(_reverseoOrder, "Reverse order");
        if (GUILayout.Button("Bind Sprite Animations"))
        {
            BindSpriteAnimations();
        }
    }

    private void BindSpriteAnimations()
    {
        string dir = _spriteAnimation.spriteFramesEditorDirectory;
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }

        string fullPath = Application.dataPath + "/" + dir;
        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(fullPath, "*.png"));
        files.Sort();
        if (_reverseoOrder)
        {
            files.Reverse();
        }
        Sprite[] sprites = new Sprite[files.Count];
        for (int i=0; i< files.Count; ++i)
        {
            string tmpPath = files[i].Replace(Application.dataPath, "Assets");
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(tmpPath);
        }

        _spriteAnimation.spriteFrames = sprites;
        Image image = _spriteAnimation.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprites[0];
        }

        EditorUtility.SetDirty(_spriteAnimation.gameObject);
    }
}

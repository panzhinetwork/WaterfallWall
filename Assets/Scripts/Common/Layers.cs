using UnityEngine;


public static class Layers
{
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int UI = LayerMask.NameToLayer("UI");

    public class Masks
    {
        public static readonly int DefaultMask = 1 << Default;
    }

    public static bool IsLayerInMask(int layer, int layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static void SetGameObjectLayer(GameObject go, int layer) {
        go.layer = layer;
        foreach (Transform child in go.transform) {
            SetGameObjectLayer(child.gameObject, layer);
        }
    }
}
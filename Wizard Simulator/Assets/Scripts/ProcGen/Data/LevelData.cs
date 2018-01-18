using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelData : UpdatableData {

    // The maps width
    public int width;
    // The maps height
    public int height;
    // The size of the border around the map
    public int borderSize = 10;

    #if UNITY_EDITOR

    protected override void OnValidate() {
        width = Mathf.Max(width, 0);
        height = Mathf.Max(height, 0);
        borderSize = Mathf.Max(borderSize, 0);
        base.OnValidate();
    }

    #endif

}

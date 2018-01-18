using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData {
    // How filled the map will be with walls/empty zones
    [Range(0, 100)]
    public int randomFillPercent;
    // How many times the map will be run through a smoothing algorithm
    public int numOfSmoothingPasses = 5;


    // This is now replaced by my object spawning data. Must find a way to have a type of prefab say snowy or dessert ect.
    // Also have an array of these prefabs to randomly chose between to give more variation
    // The floors prefab
    public GameObject groundPrefab;
    // The space bettween the ground prefabs
    public float groundSpacer = 8.0f;

    // The smallest you empty spaces can be
    public int emptyThresholdSize = 50;
    // The smallest terrain region therre can be
    public int terrainThresholdSize = 50;
    // The size of the connecting corridors
    public int pathSize = 1;

    // Which type of room connection do you want
    public enum ConnectionType { All, Closest, None };
    public ConnectionType connectionType;

    #if UNITY_EDITOR

    protected override void OnValidate() {
        groundSpacer = Mathf.Max(groundSpacer, 0.0f);
        emptyThresholdSize = Mathf.Max(emptyThresholdSize, 0);
        terrainThresholdSize = Mathf.Max(terrainThresholdSize, 0);
        pathSize = Mathf.Max(pathSize, 0);
        base.OnValidate();
    }

    #endif
}

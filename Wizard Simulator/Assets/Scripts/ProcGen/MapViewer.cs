using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewer : MonoBehaviour {

    // The place where we render our NoiseMap
    public Renderer textureRender;
    // The place where we render our Mesh
    public MeshFilter meshfilter;

    // Choose how the map is displayed
    public enum DrawMode { NoiseMap, FalloffMap, TerrainMap, Terrain }; // Level, Mesh
    public DrawMode drawMode;

    public TextureData textureData;
    public MeshSettings meshSettings;
    public LevelData levelData;
    public TerrainData terrainData;
    public HeightMapSettings heightMapSettings;

    public Material terrainMaterial;

    public bool autoUpdate;

    private bool isDirty = true;

    void Awake() {
        ClearAllChildObjects();
        isDirty = false;
    }

    void Start() {
        ClearAllChildObjects();
        isDirty = false;
    }

    public void DrawMapInEditor() {
        if (isDirty) {
            ClearAllChildObjects();
            isDirty = false;
        }
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(levelData.width, levelData.height, heightMapSettings, Vector2.zero);

        if (drawMode == DrawMode.NoiseMap)
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        else if (drawMode == DrawMode.FalloffMap)
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(HeightMapGenerator.GenerateFalloffMap(levelData.width, levelData.height), 0, 1)));
        else if (drawMode == DrawMode.TerrainMap)
            DrawTexture(TerrainGenerator.GenerateMapTexture(transform, heightMap, terrainData));
        else if (drawMode == DrawMode.Terrain)
            DrawTerrain(heightMap);
    }

    public void DrawTexture(Texture2D texture)
    {
        // Apllying the texture I created
        textureRender.sharedMaterial.mainTexture = texture;
        // Scaling the map accordingly
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10.0f;

        // Clearing any terrain pieces left in the scene
        TerrainGenerator.ClearTerrain();
        // Setting the plane to active
        textureRender.gameObject.SetActive(true);
        // Setting the mesh to in-active
        meshfilter.gameObject.SetActive(false);
    }

    public void DrawTerrain(HeightMap heightMap)
    {
        // Using my floor generator to place floor pieces base on the noise map
        TerrainGenerator.GenerateMap(transform, heightMap, terrainData);

        // Setting the plane to in-active
        textureRender.gameObject.SetActive(false);
        // Setting the mesh to in-active
        meshfilter.gameObject.SetActive(false);
    }

    public void ClearAllChildObjects() {
        for (int i = transform.childCount - 1; i >= 2; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void OnValuesUpdated() {
        // When the values are updated we are just refreshing our map
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    void OnTextureValuesUpdated() {
        // Applying our new texture to the terrain material
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate() {
        if (heightMapSettings != null) {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (terrainData != null) {
            if (!isDirty) isDirty = true;
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (levelData != null) {
            levelData.OnValuesUpdated -= OnValuesUpdated;
            levelData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null) {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator {

    private static int width;
    private static int height;

    public static int GetWidth() {
        return width;
    }

    public static int GetHeight() {
        return height;
    }

    public static void GenerateMap(Transform parent, HeightMap heightMap, TerrainData terrainData) {
        ClearTerrain();
        width = heightMap.values.GetLength(0);
        height = heightMap.values.GetLength(1);
        int[,] map = new int[width, height];

        map = ConvertMapToCellular(heightMap, terrainData);
        FloorGeneration.GenerateFloor(map, parent, terrainData);
        SpawnAllTerrain();
    }

    public static Texture2D GenerateMapTexture(Transform parent, HeightMap heightMap, TerrainData terrainData) {
        width = heightMap.values.GetLength(0);
        height = heightMap.values.GetLength(1);
        int[,] map = new int[width, height];

        map = ConvertMapToCellular(heightMap, terrainData);
        return FloorGeneration.GenerateFloorTexture(map, terrainData);
    }

    private static int[,] ConvertMapToCellular(HeightMap heightMap, TerrainData terrainData) {
        int[,] floorMap = new int[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Defining the edges as empty space / a wall. otherwise generates randomly a number between 1 and 0.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    floorMap[x, y] = 1;
                else
                    floorMap[x, y] = ((Remap(heightMap.values[x, y], heightMap.minValue, heightMap.maxValue, 0, 1) * 100) <= terrainData.randomFillPercent) ? 1 : 0;
            }
        }

        return floorMap;
    }

    private static float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float[,] ConvertMapToFloat(int[,] map) {
        float[,] floatMap = new float[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                floatMap[x, y] = map[x, y];
            }
        }

        return floatMap;
    }

    public static Vector3 CoordToWorldPoint(int x, int y, float groundSpacer, bool tiles) {
        if (tiles) {
            // This is for when I have it printing the floor tiles
            return new Vector3((-width / 2 - 0.25f + x) * groundSpacer, 4, (-height / 2 - 0.25f + y) * groundSpacer);
        } else {
            // This is for when I am printing my noise map to a texture
            return new Vector3(-width / 2 + 0.5f + x, 2, -height / 2 + 0.5f + y);
        }
    }

    private static void SpawnAllTerrain() {
        ObjectSpawner.SpawnAllObjects();
    }

    public static void ClearTerrain() {
        ObjectSpawner.DestoryAllObjects();
    }

}

[System.Serializable]
public struct Coord {
    public int tileX;
    public int tileY;

    public Coord(int x, int y)
    {
        tileX = x;
        tileY = y;
    }
}


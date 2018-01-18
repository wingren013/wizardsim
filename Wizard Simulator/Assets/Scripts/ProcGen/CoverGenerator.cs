using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoverGenerator {

    private static int width;
    private static int height;
    private static int[,] map;

    public static void GenerateFoliage(int[,] inMap, Transform parent) {

    }

    public static Texture2D GenerateFoliageMap(int[,] inMap) {
        return TextureGenerator.TextureFromHeightMap(new HeightMap(TerrainGenerator.ConvertMapToFloat(map), 0, 1));
    }

}

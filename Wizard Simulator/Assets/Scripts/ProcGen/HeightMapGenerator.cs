using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {

    /* private static HeightMapSettings settings;

    public static void SetHeightMap(HeightMapSettings mySettings) {
        settings = mySettings;
    } // If I wanted to add the ability to change Falloff settings */

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 center) {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, center);
        float[,] falloffMap = GenerateFalloffMap((settings.useFalloffMap) ? width : 0, (settings.useFalloffMap) ? height : 0);
        AnimationCurve heightCurve_ts = new AnimationCurve(settings.heightCurve.keys);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (settings.useFalloffMap)
                    values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
                values[i, j] *= heightCurve_ts.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                    maxValue = values[i, j];
                if (values[i, j] < minValue)
                    minValue = values[i, j];
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }

    public static float[,] GenerateFalloffMap(int width, int height) {
        float[,] map = new float[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;
                float val = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                map[i, j] = Evaluate(val);
            }
        }

        return map;
    }

    private static float Evaluate(float val) {
        // Tier size
        float a = 3.0f;
        // Empty size
        float b = 4.2f;

        return Mathf.Pow(val, a) / (Mathf.Pow(val, a) + Mathf.Pow(b - b * val, a));
    }

}

public struct HeightMap {
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue) {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

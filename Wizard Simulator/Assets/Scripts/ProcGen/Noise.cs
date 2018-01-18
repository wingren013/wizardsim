using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

	public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 center) {
        float[,] noiseMap = new float[width, height];
        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        float maxPossibleHeight = 0.0f;
        float amplitude = 1.0f;
        float frequency = 1.0f;

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + center.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y + center.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalHeight = float.MinValue;
        float minLocalHeight = float.MaxValue;
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float noiseHeight = 0;

                amplitude = 1.0f;
                frequency = 1.0f;
                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    float rand = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += rand * amplitude;
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalHeight)
                    maxLocalHeight = noiseHeight;
                if (noiseHeight < minLocalHeight)
                    minLocalHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
                if (settings.normalizeMode == NormalizeMode.Global) {
                    float normHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;

                    noiseMap[x, y] = Mathf.Clamp(normHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalHeight, maxLocalHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50.0f;

    public int octaves = 1;
    [Range(0, 1)]
    public float persistance = 0.6f;
    public float lacunarity = 2.0f;

    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1.0f);
        persistance = Mathf.Clamp01(persistance);
    }
}

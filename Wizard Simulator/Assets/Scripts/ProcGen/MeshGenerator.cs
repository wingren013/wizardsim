﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail) {
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int numVertsPerLine = meshSettings.numVertsPerLine;
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2.0f;
        MeshData meshData = new MeshData(numVertsPerLine, skipIncrement);
        int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        for (int y = 0; y < numVertsPerLine; y++) {
            for (int x = 0; x < numVertsPerLine; x++) {
                bool isOutOfMeshVert = (y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1);
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y < 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutOfMeshVert) {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                } else if (!isSkippedVertex) {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < numVertsPerLine; y++) {
            for (int x = 0; x < numVertsPerLine; x++) {
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex) {
                    bool isOutOfMeshVert = (y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1);
                    bool isMeshEdgeVertex = !isOutOfMeshVert && (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2);
                    bool isMainVertex = !isOutOfMeshVert && !isMeshEdgeVertex && (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0;
                    bool isEdgeConnectionVertex = !isMainVertex && !isOutOfMeshVert && !isMeshEdgeVertex && (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3);
                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));
                    int vertexIndex = vertexIndicesMap[x, y];
                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, percent.y) * meshSettings.meshWorldSize;
                    float height = heightMap[x, y];

                    if (isEdgeConnectionVertex) {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int dstToMainVertexA = ((isVertical) ? (y - 2) : (x - 2)) % skipIncrement;
                        int dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;
                        float heightMainVertexA = heightMap[(isVertical) ? x : (x - dstToMainVertexA), (isVertical) ? (y - dstToMainVertexA) : y];
                        float heightMainVertexB = heightMap[(isVertical) ? x : (x + dstToMainVertexB), (isVertical) ? (y + dstToMainVertexB) : y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }
                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);
                    if (createTriangle) {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;
                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
        }
        meshData.Finnalize();

        return meshData;
    }

}

public class MeshData {

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;

    public MeshData(int numVertsPerLine, int skipIncrement) {
        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;
        int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;

        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
        uvs = new Vector2[vertices.Length];
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];
        outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
        if (vertexIndex < 0) {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        } else {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c) {
        if (a < 0 || b < 0 || c < 0) {
            outOfMeshTriangles[outOfMeshTriangleIndex++] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex++] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex++] = c;
        } else {
            triangles[triangleIndex++] = a;
            triangles[triangleIndex++] = b;
            triangles[triangleIndex++] = c;
        }
    }

    private Vector3[] CalculateNormals() {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        int borderTriangleCount = outOfMeshTriangles.Length / 3;

        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < borderTriangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            if (vertexIndexA >= 0)
                vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0)
                vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0)
                vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void Finnalize() {
        BakeNormals();
    }

    private void BakeNormals() {
        bakedNormals = CalculateNormals();
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = bakedNormals;

        return mesh;
    }

}
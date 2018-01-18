using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    static Queue<ObjectSpawnerData> spawnList = new Queue<ObjectSpawnerData>();
    static List<GameObject> allObjects = new List<GameObject>();

    public static void QueueObjectSpawn(ObjectSpawnerData spawnData) {
        lock (spawnList) {
            spawnList.Enqueue(spawnData);
        }
    }

    public static void SpawnAllObjects() {
        Quaternion newRot = new Quaternion();
        int halfWidth = TerrainGenerator.GetWidth() / 2;
        int halfHeight = TerrainGenerator.GetHeight() / 2;

        if (allObjects.Count > 0)
            allObjects.Clear();
        while (spawnList.Count > 0) {
            ObjectSpawnerData obj = spawnList.Dequeue();
            Vector3 pos = new Vector3(obj.pos.x, obj.pos.y, obj.pos.z);

            newRot.eulerAngles = obj.rot;
            GameObject prefab = Instantiate(obj.prefab, new Vector3((pos.x - halfWidth) * obj.groundSpacer.x, pos.y * obj.groundSpacer.y, (pos.z - halfHeight) * obj.groundSpacer.z), newRot) as GameObject;
            prefab.transform.parent = obj.parent;
            prefab.transform.localScale = obj.scale;
            CheckForPlacedTiles(prefab);
        }
    }

    private static void CheckForPlacedTiles(GameObject prefab) {
        bool found = false;

        for (int i = allObjects.Count - 1; i >= 0; i--) {
            if (allObjects[i].transform.position == prefab.transform.position) {
                DestroyImmediate(prefab);
                found = true;
                break;
            }
        }
        if (!found) {
            lock (allObjects) {
                allObjects.Add(prefab);
            }
        }
    }

    public static void DestoryAllObjects() {
        for (int i = allObjects.Count - 1; i >= 0; i--) {
            DestroyImmediate(allObjects[i].gameObject);
        }
        if (allObjects.Count > 0)
            allObjects.Clear();
    }

}

[System.Serializable]
public class ObjectSpawnerData {

    // The prefab to be spawned
    public GameObject prefab;
    // The prefabs starting position
    public Vector3 pos;
    // The scale of the prefab
    public Vector3 scale = new Vector3(4.0f, 4.0f, 1.0f);
    // The rotation of the prefab (between 0 and 360)
    public Vector3 rot = new Vector3(270.0f, 0.0f, 0.0f);
    // The space bettween the prefabs
    public Vector3 groundSpacer = new Vector3(8.0f, 1.0f, 8.0f);
    // The parent of the object being spawned
    public Transform parent;

    public ObjectSpawnerData(GameObject prefab, Vector3 pos, Vector3 scale, Vector3 rot, Vector3 groundSpacer, Transform parent) {
        this.prefab = prefab;
        this.pos = pos;
        this.scale = scale;
        this.rot = rot;
        this.groundSpacer = groundSpacer;
        this.parent = parent;
    }

    public void ValidateValues() {
        scale = new Vector3(Mathf.Max(scale.x, 0.01f), Mathf.Max(scale.y, 0.01f), Mathf.Max(scale.z, 0.01f));
        rot = new Vector3(Mathf.Clamp(rot.x, 0.0f, 360.0f), Mathf.Clamp(rot.y, 0.0f, 360.0f), Mathf.Clamp(rot.z, 0.0f, 360.0f));
        groundSpacer = new Vector3(Mathf.Max(groundSpacer.x, 0.01f), Mathf.Max(groundSpacer.y, 0.01f), Mathf.Max(groundSpacer.z, 0.01f));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour {

    public GameObject player;
    public float respawnTimer = 0.0f;
    public float respawnTime = 5.0f;
    public GameObject enemyPrefab;
    GameObject spawnedEnemy = null;

	// Use this for initialization
	void Start () {
        respawnTimer = respawnTime;
	}
	
	// Update is called once per frame
	void Update () {

        if (spawnedEnemy == null)
        {
            respawnTimer -= Time.deltaTime;
        }
        if (respawnTimer <= 0.0f)
        {
            spawnedEnemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
            respawnTimer = respawnTime;
            spawnedEnemy.GetComponent<Enemy_Base>().player = player;
            spawnedEnemy.GetComponent<Enemy_Base>().target = player;
            spawnedEnemy.GetComponent<Enemy_Base>().camera = player;
        }
	}
}

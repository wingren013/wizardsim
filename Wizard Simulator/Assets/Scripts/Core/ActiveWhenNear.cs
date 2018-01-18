using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWhenNear : MonoBehaviour {

    public float activationDst;
    public GameObject player;
	
	// Update is called once per frame
	void Update () {
        // find the distance between this object and the player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= activationDst)
            this.gameObject.SetActive(true);
        else
            this.gameObject.SetActive(false);
    }
}

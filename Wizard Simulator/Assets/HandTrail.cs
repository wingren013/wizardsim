using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class HandTrail : MonoBehaviour {

    public Hand hand;
    public GameObject trail;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(hand.GetTrackedObjectVelocity().magnitude > .1f)
        {
            trail.SetActive(true);
        }
        else
        {
            trail.SetActive(false);
        }

	}
}

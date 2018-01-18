using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Fist : MonoBehaviour {

    public Hand hand;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("You hit something with your hand with a magnitude of " + hand.GetTrackedObjectVelocity().magnitude);
        GameObject hitObject;
        if (other.attachedRigidbody != null)
            hitObject = other.attachedRigidbody.gameObject;
        else
            hitObject = other.gameObject;
        Health health = hitObject.GetComponent<Health>();
        if (health && hand.GetTrackedObjectVelocity().magnitude > 1.0f)
        {
            health.DoDamageLocation(5.0f, Health.DamageType.Physical, this.transform.position);
            hand.controller.TriggerHapticPulse();
            this.GetComponent<AudioSource>().Play();
        }
    }
}

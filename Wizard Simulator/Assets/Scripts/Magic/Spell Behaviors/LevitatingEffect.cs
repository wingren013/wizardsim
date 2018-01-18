using UnityEngine;
using System.Collections;
using System;

public class LevitatingEffect : SpellEffect {
	private MeshFilter meshFilter;

    void Awake()
    {
       
        defaultDuration = 6.0f;
    }

	// Use this for initialization
	public override void EffectStart () {
		meshFilter = GetComponent<MeshFilter> ();
		if (GetComponent<Rigidbody>()) 
		{
			GetComponent<Rigidbody>().useGravity = false;
		}
		if (gameObject.GetComponent<TimeEffect> () != null)
			gameObject.GetComponent<TimeEffect> ().originalGravitySetting = false;
	}
	
	// Update is called once per frame
	void OnDestroy(){
		if (GetComponent<Rigidbody>()) 
		{
			GetComponent<Rigidbody>().useGravity = true;
		}

	}


    public override void Dispel()
    {

    }
}

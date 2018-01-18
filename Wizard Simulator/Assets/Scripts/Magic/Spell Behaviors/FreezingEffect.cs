using UnityEngine;
using System.Collections;
using System;

public class FreezeEffect : SpellEffect {
	
	public float freezetime = 5.0f;
	public float warmtime = 5.0f; 
	public Color chillcolor = Color.cyan;
	private Color startColor;
	private Material startMaterial;
	private MeshFilter meshFilter;
	private Collider propogationCollider;
	private GameObject freezingObject;

	private ParticleSystem iceparticles;
	
	// Use this for initialization
	void Start () {

		if (GetComponent<MissileShape>() != null) 
		{

		}
		else if (GetComponent<RayShape>() != null) 
		{

		}

		
	}

	// Update is called once per frame
	public override void EffectUpdate () {


		if( freezetime > 0.0f)
		{
			freezetime -= Time.deltaTime;
			if(freezetime <= 0.0f)
			{
				if(meshFilter != null)
				{
					meshFilter.GetComponent<Renderer>().material = startMaterial;
					meshFilter.GetComponent<Renderer>().material.color = chillcolor;
				}
				iceparticles.enableEmission = false;

			}
		}
		else if(freezetime <= 0.0f && warmtime > 0.0f)
		{
			warmtime -=Time.deltaTime;
			//meshFilter.renderer.material.color += chardelta*Time.deltaTime;
		}
		else
		{
			if(meshFilter != null)
				meshFilter.GetComponent<Renderer>().material.color = startColor;
			Destroy(this);
		}
		
	
	}
    //Dispell
    public override void Dispel()
    {

    }
}
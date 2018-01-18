using UnityEngine;
using System.Collections;
using System;

public class BurningEffect : SpellEffect {

    //Reference to the fire emitting particle object
    static private GameObject flamePrefab;
    private GameObject flameObject;

	void Awake()
	{
        if(flamePrefab == null)
        {
            flamePrefab = Resources.Load("Effects/Burning Particles") as GameObject;
        }
        isPropagator = true;
        jumpsRemaining = 1;
        damageType = Health.DamageType.Burn;
        doesDamage = true;
        damageOverTime = 100.0f;
        defaultDuration = 5.0f;
    }

    public override void EffectStart()
    {
        flameObject = Instantiate(flamePrefab, this.transform);
        var shape = flameObject.GetComponent<ParticleSystem>().shape;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        if(this.GetComponentInChildren<MeshFilter>())
          shape.mesh = this.GetComponentInChildren<MeshFilter>().mesh;
        flameObject.SetActive(true);
    }

    // Update is called once per frame
    public override void EffectUpdate ()
    {
        	
	}

    public override void Dispel()
    {
        Destroy(flameObject);
    }

    void OnDestroy()
	{
	
	}
}
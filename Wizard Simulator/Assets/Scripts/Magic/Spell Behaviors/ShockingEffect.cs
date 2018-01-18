using UnityEngine;
using System.Collections;
using System;

public class ShockingEffect : SpellEffect
{

    //Reference to the fire emitting particle object
    static private GameObject shockPrefab;
    private GameObject shockObject;

    void Awake()
    {
        if (shockPrefab == null)
        {
            shockPrefab = Resources.Load("Effects/Shocking Particles") as GameObject;
        }
        isPropagator = true;
        jumpsRemaining = 1;
        damageType = Health.DamageType.Shock;
        doesDamage = true;
        damageBurst = 20.0f;
        damageOverTime = 0.0f;
        defaultDuration = 1.0f;
    }

    public override void EffectStart()
    {
        shockObject = Instantiate(shockPrefab, this.transform);
        var shape = shockObject.GetComponent<ParticleSystem>().shape;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        MeshFilter mf = this.GetComponentInChildren<MeshFilter>();
        if(mf != null)
            shape.mesh = this.GetComponentInChildren<MeshFilter>().mesh;
        shockObject.SetActive(true);
    }

    // Update is called once per frame
    public override void EffectUpdate()
    {

    }

    public override void Dispel()
    {
        Destroy(shockObject);
    }

    void OnDestroy()
    {

    }
}
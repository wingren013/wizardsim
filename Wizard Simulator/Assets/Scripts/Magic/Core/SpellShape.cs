using System.Collections.Generic;
using UnityEngine;

/*
 * [SpellShape Class]
 * 
 * Spellshapes are the fundamental building blocks of spellcasting.  
 * They carry and transfer SpellEffects and act as spellcasters
 * that propogate further spells.
 * 
 * Spellshapes have default origin points designating where
 * its chained spell will be cast from.
 */

public abstract class SpellShape : Spell {

    public enum CastOrigin
    {
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back,
        Default
    }

    public enum SpellShapeType
    {
        Missile,
        Ray,
        Bubble
    }

    protected SpellShapeType spellShapeType;

    //The spell that this spell will cast
    public GameObject chainedSpell;

    //The spell that this spell was cast from
    public GameObject parentSpell;

    //The name of the relative point from which this spell will cast further spells
    public CastOrigin castOrigin = CastOrigin.Default;


    //Booleans for when to cast
    public bool castOnDispel = false;
    public bool castOnRepeat = false;

    //Frequency of casting chained spell.
    public float castFrequency = 0.0f;

    //Time since spell has cast its chained spell
    public float castTimer = 0.0f;

    //Time to charge this spell if it is the primary parent spell
    public float castTime = 1.0f;

    //A table of points from which spells can be cast relative to this one
    public Dictionary<CastOrigin, Vector3> castOrigins;

    //Public accessor for shape type
    public SpellShapeType GetShapeType()
    {
        return this.spellShapeType;
    }

    //Public accessor for cast time
    public float GetCastTime()
    {
        return castTime;
    }

    //Chains spell to this spell
    public virtual void ChainSpell(GameObject spell)
    {
        chainedSpell = spell;
    }

    //Sets spell origin
    public void SetCastOrigin(CastOrigin origin)
    {
        this.castOrigin = origin;
    }

    //Sets spell's spellcaster
    public void SetParentSpell(GameObject parent)
    {
        this.parentSpell = parent;
    }

    public SpellShape()
    {
        chainedSpell = null;
    }

    //Spawns the chained spell
    public GameObject SpawnSpell()
    {
        Vector3 spawn_point;

        //If there's no list of origins, cast it at a default position.
        //Otherwise instantiate it at the designated spawn point.
        //
        //We will eventually want to ascertain which spellshape we're spawning
        //And have object pools that spawn the appropriate SpellShape

        if (castOrigins == null || castOrigins[chainedSpell.GetComponent<SpellShape>().castOrigin] == null)
            spawn_point = transform.position;
        else
            spawn_point = transform.position + (transform.rotation * castOrigins[chainedSpell.GetComponent<SpellShape>().castOrigin]);
        GameObject newSpell = Instantiate(chainedSpell, spawn_point, Quaternion.identity) as GameObject;
        newSpell.GetComponent<SpellShape>().SetParentSpell(newSpell);
        return newSpell;
    }

    //Trigger event hook interface  Implement the abstract methods in derived shapes to add trigger behavior.

    public virtual void ShapeTriggerEnter(Collider other) { }

    private void OnTriggerEnter(Collider other)
    {
        ShapeTriggerEnter(other);
    }

    public virtual void ShapeTriggerExit(Collider other) { }

    private void OnTriggerExit(Collider other)
    {
        ShapeTriggerExit(other);
    }

    public virtual void ShapeTriggerStay(Collider other) { }

    private void OnTriggerStay(Collider other)
    {
        ShapeTriggerStay(other);
    }

    //Abstract method for casting spells.  Should use SpawnSpell() to create a copy of the chained
    //spell at the designated location 
    public abstract void CastSpell();

    public override void SpellStart()
    {
        castOrigins = new Dictionary<CastOrigin, Vector3>
        {
            { CastOrigin.Default, Vector3.zero },
            { CastOrigin.Top, Vector3.up },
            { CastOrigin.Bottom, Vector3.down },
            { CastOrigin.Left, Vector3.left },
            { CastOrigin.Right, Vector3.right },
            { CastOrigin.Front, Vector3.forward },
            { CastOrigin.Back, Vector3.back},

        };
        castOrigin = CastOrigin.Default;
        ShapeStart();
    }

    //Abstract method for shape specific start code
    public abstract void ShapeStart();

    //Abstract method for shape specific update
    public abstract void ShapeUpdate();

    //Called by Spell.Update() once per frame
    public override void SpellUpdate()
    {
        if(castOnRepeat && castFrequency > 0.1f)
        {
            castTimer -= Time.deltaTime * timeScale;
            if(castTimer <= 0.0f)
            {
                CastSpell();
                castTimer = castFrequency;
            }
        }
        ShapeUpdate();
    }

    //Virtual method for cleanup.  Override for spell specific cleanup procedure.
    //Defaults to destroying the object of the spellshape.
    public override void Dispel()
    {
        ShapeDispel();

        for (int i = 0; i < transform.childCount; i++)
        {
            SpellShape sh = transform.GetChild(i).GetComponent<SpellShape>();
            if (sh != null)
                sh.Dispel();
        }
    }

    public virtual void ShapeDispel()
    {
		if (castOnDispel)
			CastSpell();
        Destroy(this.gameObject);
    }
}

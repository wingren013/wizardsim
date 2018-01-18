using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BubbleShape : SpellShape
{
	public Vector3 original_scale;
    public GameObject model;
	public float grow_speed = 5.1f;
	public Vector3 start_scale = new Vector3(.1f,.1f,.1f);
	public bool expired = false;

    public BubbleShape()
    {
        spellShapeType = SpellShapeType.Bubble;
    }

    public void Awake()
    {
        castFrequency = 0.3f;
		if (chainedSpell)
		{
			if (chainedSpell.GetComponent<SpellShape>().GetShapeType() == SpellShapeType.Bubble)
			{
				castFrequency = 0.0f;
			}
		}
    }

	public override void ShapeStart()
	{
        permanent = false;
        original_scale = model.transform.localScale;
        model.transform.localScale = start_scale;
        spellShapeType = SpellShapeType.Bubble;
	}

	public override void ShapeUpdate()
	{
		if (model.transform.localScale != original_scale && duration >= 1.0f)
			Grow ();
		else if (duration < 1.0f)
			Shrink ();
	}

	public override void CastSpell()
	{
		if (!chainedSpell)
			return;
        GameObject newspell = SpawnSpell();
        //newspell.SetActive(true);
        switch (newspell.GetComponent<SpellShape>().GetShapeType()) 
		{
		case SpellShapeType.Missile:
				newspell.transform.parent = gameObject.transform;
				newspell.transform.position = (Random.insideUnitSphere * gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x) + gameObject.transform.position;
				newspell.transform.localRotation = Quaternion.LookRotation(newspell.transform.localPosition);
				// move the missile forward slightly
				newspell.transform.position += newspell.transform.forward * newspell.GetComponent<MissileShape>().getRadius();
				castFrequency = 0.2f;
				newspell.GetComponent<Rigidbody>().velocity = newspell.transform.localPosition * 10;
                newspell.transform.parent = null;
				newspell.SetActive(true);
				break;
		case SpellShapeType.Ray:
				newspell.transform.parent = gameObject.transform;
				newspell.transform.position = (Random.insideUnitSphere * gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x) + gameObject.transform.position;
				newspell.transform.localRotation = Quaternion.LookRotation(newspell.transform.localPosition);
				newspell.GetComponent<SpellShape>().duration = 1.3f;
				newspell.SetActive(true);
				break;
		case SpellShapeType.Bubble:
				newspell.transform.position = gameObject.transform.position;
				//newspell.transform.position = (Random.insideUnitSphere * gameObject.GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x) + gameObject.transform.position;
				newspell.transform.localRotation = Quaternion.LookRotation(newspell.transform.localPosition);
				newspell.SetActive(true);
				break;
		}
	}


    //Sets the color based on the primary effect.

    private void ResolveVisual()
    {

    }

    //Grows bubble to target size.

    private void Grow()
	{
        model.transform.localScale = Vector3.Lerp(model.transform.localScale,original_scale,Time.deltaTime*grow_speed);
	}

    //Shrinks bubble down.
	
	private void Shrink()
	{
        model.transform.localScale = Vector3.Lerp(model.transform.localScale,start_scale,Time.deltaTime*grow_speed);
	}
}

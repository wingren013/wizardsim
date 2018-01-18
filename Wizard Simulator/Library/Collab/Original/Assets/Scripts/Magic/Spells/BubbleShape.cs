using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BubbleShape : SpellShape
{
	public Vector3 original_scale;
	public float grow_speed = 5.1f;
	public Vector3 start_scale = new Vector3(.1f,.1f,.1f);
	public bool expired = false;

    BubbleShape()
    {
        spellShapeType = SpellShapeType.Bubble;
    }
	public void Awake()
	{
		CastSpell();
		castFrequency = 0.3f;
	}
	public override void ShapeStart()
	{
        permanent = false;
		original_scale = transform.localScale;
		transform.localScale = start_scale;
		castOrigins = new Dictionary<CastOrigin, Vector3>();
		castOrigins.Add(CastOrigin.Top, Vector3.up * getRadius() * 3);
		castOrigins.Add(CastOrigin.Bottom, Vector3.down * getRadius() * 3);
		castOrigins.Add(CastOrigin.Left, Vector3.left * getRadius() * 3);
		castOrigins.Add(CastOrigin.Right, Vector3.right * getRadius() * 3);
		castOrigins.Add(CastOrigin.Front, Vector3.forward * getRadius() * 3);
		castOrigins.Add(CastOrigin.Back, Vector3.back * getRadius() * 3);
		castOrigins.Add(CastOrigin.Default, Vector3.zero);
	}

	public override void ShapeUpdate()
	{
		if (transform.localScale != original_scale && duration >= 1.0f)
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
				newspell.transform.localRotation = Quaternion.LookRotation(castOrigins[castOrigin]);
				newspell.transform.position += newspell.transform.forward * newspell.GetComponent<MissileShape>().getRadius();
				break;
		case SpellShapeType.Ray:
				newspell.transform.parent = gameObject.transform;
				newspell.transform.position = transform.position;
				Debug.Log(castOrigin);
				Debug.Log(newspell);
				Debug.Log(newspell.transform.localRotation);
				Debug.Log(castOrigins);
				newspell.transform.localRotation = Quaternion.LookRotation(castOrigins[castOrigin]);
				newspell.SetActive(true);
				break;
		case SpellShapeType.Bubble:
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
		transform.localScale = Vector3.Lerp(transform.localScale,original_scale,Time.deltaTime*grow_speed);
	}

    //Shrinks bubble down.
	
	private void Shrink()
	{
		transform.localScale = Vector3.Lerp(transform.localScale,start_scale,Time.deltaTime*grow_speed);
	}
}

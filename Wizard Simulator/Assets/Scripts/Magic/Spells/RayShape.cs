using UnityEngine;

public class RayShape : SpellShape
{
    public LayerMask myLayerMask;

    //This is the game object holding the beam model.
    public GameObject beamObject;

    //This is an empty game object with a transform at the tip of the beam.  
    //Don't parent anything to this.
    public GameObject endObject;

    //This is also at the tip of the beam, it's a sphere with a collider for doing damage.  
    //Anything contact/damage related should be hitting this and triggering a trigger-enter-event
    public GameObject hitObject;

    //This is just a variable to hold chained spells that are childed like bubbles/other beams.  
    //Missiles don't go here.
    public GameObject contactSpell;

    //Length of the ray
    private float maxLength = 100.0f;
    private float length = 0.0f;

    //Cooldown for reapplying damage and effects
    private float coolDownDuration = 0.5f;
    private float coolDownTimer = 0.0f;

    public RayShape()
    {
        spellShapeType = SpellShapeType.Ray;
    }

    public override void ShapeStart()
    {
        CastSpell();
    }

    public void Awake()
    {
        castOnDispel = false;
    }

    public override void ShapeUpdate()
    {
        //Perform a ray cast and adjust the z scale of the beam object.  Pretty simple.
        RaycastHit hit = new RaycastHit();

        if (coolDownTimer > 0.0f)
            coolDownTimer -= Time.deltaTime;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f, myLayerMask))
        {
            hitObject.SetActive(true);
            //This is to start bubble/child activation on contact
            if (contactSpell != null)
            {
                if (contactSpell.GetComponent<SpellShape>().GetShapeType() == SpellShapeType.Ray)
                {
                    contactSpell.transform.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.forward, hit.normal));
                }
                contactSpell.SetActive(true);
            }
            length = Vector3.Magnitude(hit.point - transform.position);
            hitObject.transform.position = hit.point;

            Health h = hit.collider.gameObject.GetComponent<Health>();
            //This deals with damage/effects
            if (h != null)
            {
                if(coolDownTimer <= 0.0f)
                {
                    coolDownTimer = coolDownDuration;
                    foreach(SpellEffect s in GetComponents<SpellEffect>())
                        s.Propogate(h.gameObject);
                }
            }
        }
        else
        {
            length = maxLength;
            hitObject.transform.position = endObject.transform.position;
            hitObject.SetActive(false);
            //This is to stop bubble/child activation on lost contact
            if (contactSpell != null)
                contactSpell.SetActive(false);
        }

        //For ray/ray ray/bubble.  Causes the bubble to follow the end of the ray.
        if(contactSpell)
        {
			if (hitObject.activeSelf)
				contactSpell.transform.position = endObject.transform.position;
        }

        beamObject.transform.localScale = new Vector3(beamObject.transform.localScale.x, beamObject.transform.localScale.y, length * .5f);
    }

    public override void CastSpell()
    {
		if (!chainedSpell)
			return;
        GameObject newspell = SpawnSpell();
        
        switch (newspell.GetComponent<SpellShape>().GetShapeType())
        {
            case SpellShapeType.Bubble:
                contactSpell = newspell;
                break;
            case SpellShapeType.Ray:
                contactSpell = newspell;
                break;
            case SpellShapeType.Missile:
                Debug.Log("FIRING MISSILE I GUESS");

                newspell.GetComponent<MissileShape>().SetHoming(true, endObject.transform);
                newspell.transform.LookAt(endObject.transform);
                break;
        }

        newspell.SetActive(true);
    }

    public override void Dispel()
    {
        base.Dispel();
    }
}
using UnityEngine;
using System.Collections.Generic;

public class MissileShape : SpellShape {
    private bool  _dead = false;
    private float _targetDelta = 0.0f;
    private float _homingSpeed = 5.0f;
    private float _minHomingSpeed = 1.0f;
    private float _maxHomingSpeed = 10.0f;
    private float _homingAcceleration = 0.1f;

    private Vector3 lastPosition;
    public Quaternion heading;
    public GameObject explosion;
    private bool homing = false;
	public Transform target;

    public MissileShape()
	{
        heading = new Quaternion();
        spellShapeType = SpellShapeType.Missile;
	}

	public override void ShapeStart ()
    {
        lastPosition = transform.position;
    }

    public void Awake()
    {
        if (chainedSpell != null)
        {
            SpellShapeType t = chainedSpell.GetComponent<SpellShape>().GetShapeType();
            if (t == SpellShapeType.Ray)
            {
                castOnRepeat = false;
                castOnDispel = false;
                CastSpell();
            }
            else if (t == SpellShapeType.Missile)
            {
                castOnRepeat = true;
                castOnDispel = false;
                castFrequency = 0.3f;
            }
            else if (t == SpellShapeType.Bubble)
            {
                castOnRepeat = false;
                castOnDispel = true;
            }
        }
        //Turns spell into a frequency-fire spellcaster.
    }

	public void SetHoming(bool set, Transform target)
	{
		homing = set;
        GetComponent<Rigidbody>().useGravity = false;
        this.target = target;
	}

	public override void ShapeUpdate()
	{
        //If it's a homing missile, follow the target.  Otherwise just follow the velocity vector.
        if (homing == true && target != null)
        {
            //Did it get closer or farther from the target?  That's our delta.
            _targetDelta = Vector3.Distance(target.position, lastPosition) - Vector3.Distance(target.position, transform.position);

            //If it's positive, speed up, otherwise slow down
            if (_targetDelta > 0.0f)
            {
                _homingSpeed += _homingAcceleration * Time.deltaTime * timeScale;
                if (_homingSpeed > _maxHomingSpeed)
                    _homingSpeed = _maxHomingSpeed;
            }
            else if (_targetDelta < 0.0f)
            {
                _homingSpeed -= _homingAcceleration * Time.deltaTime * timeScale;
                if (_homingSpeed < _minHomingSpeed)
                    _homingSpeed = _minHomingSpeed;
            }
            Debug.Log("Vector pos: " + transform.position + "Vector target: " + target.position);

            Quaternion q = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 90.0f * Time.deltaTime);
            this.GetComponent<Rigidbody>().velocity = transform.forward * _homingSpeed;
        }
        else
        {
            heading.SetLookRotation(GetComponent<Rigidbody>().velocity);
            GetComponent<Rigidbody>().rotation = heading;
        }
        lastPosition = transform.position;
    }

    
	public override void ShapeTriggerEnter(Collider collider)
	{
        GameObject hitObject;
        if (collider.attachedRigidbody != null)
            hitObject = collider.attachedRigidbody.gameObject;
        else
            hitObject = collider.gameObject;

        if (!_dead)
        {

           
            Debug.Log(hitObject.name + " was hit by " + (GetComponent<SpellEffect>() != null ? GetComponent<SpellEffect>().damageType : Health.DamageType.Arcane) + " missile!" );

            if (hitObject.tag == "Spell")
				return;
			if(hitObject.tag == "Player")
				return;
			_dead = true;


            foreach(SpellEffect s in this.GetComponents<SpellEffect>())
            {
                if(s.isPropagator)
                {
                    s.Propogate(hitObject);
                }
            }
            
            Dispel();
		}
	}

	void Explode()
	{
        explosion.transform.SetParent(null);
        explosion.SetActive(true);
        
        float radius = magnitude;

		Collider[] colliders = Physics.OverlapSphere (this.transform.position,radius);

		foreach (Collider hit in colliders)
        {
			if (hit.attachedRigidbody != null) {
                hit.attachedRigidbody.AddExplosionForce (1000.0f, this.transform.position, radius);
			}
		}
	}

	public float getRadius()
	{
		return gameObject.transform.localScale.x * gameObject.GetComponent<SphereCollider> ().radius;
	}

    //Here's where the magical combinations happen!

	public override void CastSpell()
	{
        if (chainedSpell == null)
            return;
		GameObject newspell = SpawnSpell();
		newspell.SetActive (true);
		switch (newspell.GetComponent<SpellShape>().GetShapeType()) 
		{
		case SpellShapeType.Missile:
			newspell.transform.localRotation = Quaternion.LookRotation(castOrigins[castOrigin]);
			newspell.transform.position += newspell.transform.forward * newspell.GetComponent<MissileShape>().getRadius();
			break;
		case SpellShapeType.Ray:
			newspell.transform.parent = gameObject.transform;
            newspell.transform.localRotation = Quaternion.identity;
            break;
		}
	}

    public override void ShapeDispel()
    {
        if (chainedSpell != null)
        {
            if (chainedSpell.GetComponent<SpellShape>().GetShapeType() == SpellShapeType.Bubble)
                CastSpell();
        }
        Explode();
        Destroy(this.gameObject);
    }
}

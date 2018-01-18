using UnityEngine;
using System.Collections;

public class TimeEffect : SpellEffect
{
	private Rigidbody rigidbody;
	public bool originalGravitySetting;
	private float originalMass;
	public float originalTimeScale;
	public Vector3 newGravity;

    public void Awake()
    {
        defaultDuration = 5.0f;
    }

	// Use this for initialization
	public override void EffectStart () {

        if (isPropagator)
            return;
		newGravity = new Vector3(0,Physics.gravity.y * Mathf.Pow(magnitude,2),0);
		rigidbody = GetComponent<Rigidbody> ();


        //This slows down rigidbodies by multiplying their velocity and
        // substituting gravity with our own modified vector
        if (rigidbody) 
		{
            if (rigidbody.isKinematic == false)
            {
                originalMass = rigidbody.mass;
                originalGravitySetting = rigidbody.useGravity;
                if (rigidbody.useGravity == true)
                    rigidbody.useGravity = false;
                rigidbody.mass = rigidbody.mass / magnitude;
                rigidbody.velocity = rigidbody.velocity * magnitude;
            }
		}

        
        if (GetComponent<Enemy_Base>())
        {
            originalTimeScale = GetComponent<Enemy_Base>().timeScale;
            GetComponent<Enemy_Base>().timeScale = magnitude;
            GetComponent<Enemy_Base>().agent.speed *= magnitude;
            GetComponent<Enemy_Base>().agent.velocity *= magnitude;
        }

        //This should slow down animators or speed them up.
        foreach (Animator a in GetComponentsInChildren<Animator>())
        {
            Debug.Log("Found animator- modifying timescale to " + magnitude);
            a.speed = magnitude;
        }
	}

	void OnDestroy(){
		Debug.Log("Destroying TimeEffect!");

        if (isPropagator)
            return;
        //Reinstate original velocity/gravity;
		if (rigidbody) 
		{
			rigidbody.velocity = rigidbody.velocity * (1.0f / magnitude);
			rigidbody.useGravity = originalGravitySetting;
			rigidbody.mass = originalMass;
		}

        //
        foreach (Animator a in GetComponentsInChildren<Animator>())
        {
            a.speed = 1.0f;
        }

        if(GetComponent<Enemy_Base>())
        {
            GetComponent<Enemy_Base>().timeScale = originalTimeScale;
        }
    }

	// Update is called once per frame
	void FixedUpdate () 
	{
        if (isPropagator)
            return;
		if(originalGravitySetting==true && rigidbody)
			rigidbody.AddForce(newGravity, ForceMode.Acceleration);
	}

    public override void Dispel()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandWizard : Enemy_Base {

	public Transform pos; //player's position
	public float spellcooldown = 15.0f; //for spellcasting
	public float strafe_timer = 1.0f;
	bool start = true; //for debug
					   // Use this for initialization
	void Start() {
		Init();
		if (!spellgun)
			spellgun = transform.Find("Head").transform.Find("SpellGun");
		if (!generator)
			generator = GetComponent<SpellGenerator>();
		human = true;
		movespeed = 4f;
		moveaccel = 3.1f;
		rotspeed = 360;
		if (!target)
			target = camera;
		if (!pos)
			pos = camera.transform;
		agent.acceleration = moveaccel;
		agent.angularSpeed = 360;
		agent.destination = camera.transform.position;
		//this.dodgepoints[0] = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
	}

	// Update is called once per frame
	void Update() {
		//debugging stuff
		if (start)
		{
			this.dodgepoints = new Vector3[1];
			this.dodgepoints[0] = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
			Moveto(target);
			start = false;
		}
		if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 12 && !lock_move) { near_enemy = true; }
		else if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 15) { near_enemy = true; }
		else { near_enemy = false; }

		if (near_enemy && !lock_move) { agent.speed = 0; }
		else if (!lock_move) { agent.speed = movespeed * timeScale; }

		if (strafe_timer > 0)
		{
			strafe_timer -= Time.deltaTime * this.timeScale;
		}
		else
		{
			if (near_enemy)
			{
				lock_move = true;
				Strafe();
			}
			else
			{
				lock_move = false;
				agent.destination = target.transform.position;
			}
			strafe_timer = 1.0f;
		}

			gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), rotspeed * Time.deltaTime);
            gameObject.transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

        /*
		else
		{
			if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 15)
			{
				float f = Mathf.Sqrt(Mathf.Pow(target.transform.position.x - this.gameObject.transform.position.x, 2) + Mathf.Pow(target.transform.position.z - this.gameObject.transform.position.z, 2));
				this.spellgun.Rotate(SpellChooser.CalcMissileAngle(f, 10), 0, 0);
				SpellCast("burning missile", this.gameObject.transform);
			}
			else
			{
				this.spellgun.localRotation.eulerAngles.Set(0, 0, 0);
				SpellCast("beam missile", this.gameObject.transform);
			}
			spellcooldown = 5.0f;
			//BehaviorScript.Dodge(this.dodgepoints, ref agent, 10f, 50f);
		}
        */
	}

    public void ScaleTime(float scale)
    {

    }
}
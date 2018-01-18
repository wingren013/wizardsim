using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elementalist : Enemy_Base {

	public enum State
	{
		Fire_Stage,
		Ice_Stage,
		Lightning_Stage,
		Miniboss,
	}
	public List <Hideable> hidey_holes;

	// Use this for initialization
	void Start () {
		Init();
		if (!spellgun)
			spellgun = transform.Find("Head").transform.Find("SpellGun");
		if (!generator)
			generator = GetComponent<SpellGenerator>();
		human = true;
		affected_by_fear = false;
		fire_aligned = true;
		ice_aligned = true;
		air_aligned = true;
		movespeed = 4f;
		moveaccel = 3.1f;
		rotspeed = 360;
		if (!target)
			target = camera;
		agent.acceleration = moveaccel;
		agent.angularSpeed = 360;
		keepAwayDist = 2;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("loop");
		Vector3 playerDirec = player.transform.position - gameObject.transform.position;
		if ((playerDirec).magnitude <= keepAwayDist)
			Moveto(gameObject.transform.position - playerDirec);
		//check if we are hidden
		while (true)
		{
			Debug.Log("hmmm");
			if (!CheckCover())
			{
				Debug.Log("not in cover");
				//check if we can hide behind an obstacle and move there if we can.
				if (FindCover())
					break;
				//if we can't hide we should still shoot lightning and move around
				Debug.Log("didn't find any");
				LightningBolt();
				Strafe();
				break;
			}
			else //lightnig bitches
			{
				Debug.Log("Bitches love lightning");
				CallLightning();
				break;
			}
		}
	}

	void LightningBolt()
	{
		SpellCast("shocking levitating missile", gameObject.transform);
	}

	void CallLightning()
	{
		//SpellCast("shocking curse", gameObject.transform);
	}

	protected override void Strafe()
	{
		//side to side movement when near player.
		Vector3 strafe_dest = gameObject.transform.position;
		Vector3 strafe_direction = player.transform.position - gameObject.transform.position;
		float temp;

		if (Random.value < 0.5)
		{
			temp = strafe_direction.x;
			strafe_direction.x = -1 * strafe_direction.y;
			strafe_direction.y = temp;
		}
		else
		{
			temp = -1 * strafe_direction.x;
			strafe_direction.x = strafe_direction.y;
			strafe_direction.y = temp;
		}
		strafe_dest += strafe_direction;
		agent.speed = movespeed;
		Moveto(strafe_dest);
	}
}

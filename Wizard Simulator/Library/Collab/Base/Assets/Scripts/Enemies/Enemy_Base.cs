using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : WizSimBehavior {
    //acts as a parent class for enemy controllers to ensure all enemies share some basic attributes and can be interactred with agnostic of enemy type.
    public GameObject player;
	public GameObject target;
    public GameObject camera;
	public Transform spellgun;
	public GameObject loadedSpell;
	public GameObject projectile;
	public SpellGenerator generator;

	//These attributes can be checked or modified by spells or behaivors
	public float fireres = 1;  //values under 1 represent resisitance, values over 1 represent vulnerability. Use as a coefficient to multiply damage by. Negative values will heal.
    public float coldres = 1;
    public float elecres = 1;
    public float lightres = 1;

    //creature type. 
    public bool magic_creature;
    public bool demon;
    public bool undead;
    public bool human;
    public bool fairy;
    public bool construct;

    //for stuff that cares about elements
    public bool fire_aligned = false;
    public bool ice_aligned = false;
    public bool air_aligned = false;
    public bool light_aligned = false;

    //is the enemy currently on fire or something?
    public bool burning;
    public bool frozen;
    public bool illuminated;
    public bool afraid;

    //pretty gorram self explainable.
    public bool affected_by_burn = true;
    public bool affected_by_freeze = true;
    public bool affected_by_light = true;
    public bool affected_by_fear = true;
	public bool affected_by_stun = true;

    //attributes
    public float mana;
	public float maxhp;

	public float movespeed;
    public float moveaccel;
    public float runspeed;
    public float runaccel;
    public float rotspeed;

    public float attack_damage_melee;
    public float attack_damage_ranged;
    public float attack_range; //for non spellcasting ranged enemies. Set range to 1 for melee and to 0 for spellcasters.
	public float keepAwayDist;

    //these are for internal use, but free to be modified by other scripts
    public string[] spelllist;
    public Vector3[] dodgepoints;
	protected RaycastHit hit;

	public bool near_enemy;
	public bool lock_move;

    public float timer_general;
    public float timer_move;
    public float timer_burning;
    public float timer_frozen;
    public float timer_illuminated;

    //all enemies should have a navmesh agent and be able to use genericbehaivor
    public UnityEngine.AI.NavMeshAgent agent;

    //run this in an enemy controllers onStart() function
    protected void Init()
    {
        this.player = GameObject.Find("Player");
        //this.camera = GameObject.Find("VRCamera");

        //this.dodgepoints = new Vector3[4];

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

	//--------------------------------------------
	//----Below this line are GenericBehaivors----
	//--------------------------------------------

	public void Die()
	{
		gameObject.SetActive(false);
	}

	//check cover
	public bool CheckCover()
	{
		Debug.Log("Raycasting!");
		Debug.DrawRay(gameObject.transform.position, player.transform.position - gameObject.transform.position);
		if (Physics.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			return (hit.collider.gameObject.tag != "player");
		}
		return (false);
	}

	public bool CheckCover(Vector3 raycastOffset)
	{
		Debug.Log("Raycasting!");
		Debug.DrawRay(gameObject.transform.position + raycastOffset, player.transform.position - (gameObject.transform.position + raycastOffset));
		if (Physics.Raycast(gameObject.transform.position + raycastOffset, player.transform.position - (gameObject.transform.position + raycastOffset), out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			return (hit.collider.gameObject.tag != "player");
		}
		return (false);
	}

	public bool CheckCover(Vector3 position, Vector3 raycastOffset)
	{
		Debug.Log("Raycasting!");
		Debug.DrawRay(gameObject.transform.position + raycastOffset, player.transform.position - (gameObject.transform.position + raycastOffset));
		if (Physics.Raycast(position + raycastOffset, player.transform.position - (gameObject.transform.position + raycastOffset), out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			return (hit.collider.gameObject.tag != "player");
		}
		return (false);
	}

	public bool FindCover()
	{
		Collider[] cover = Physics.OverlapSphere(gameObject.transform.position, 100, 1 << 16);

		foreach (Collider obstacle in cover)
		{
			//if our cover is more than 10 away from the player it is valid
			if ((obstacle.transform.position - player.transform.position).magnitude > keepAwayDist)
			{
				//20% chance to choose any piece of cover
				if (Random.value > 0.2)
				{
					//move a distance of two in a direction the reverse of the direction towards the player
					Moveto(obstacle.transform.position - (1.5f * (player.transform.position - gameObject.transform.position).normalized));
					return (true);
				}
			}
		}
		return (false);
	}

	//store the cover's position in a reference rather than moving to it. 
	public bool FindCover(ref Vector3 goal)
	{
		Collider[] cover = Physics.OverlapSphere(gameObject.transform.position, 100, 1 << 16);

		foreach (Collider obstacle in cover)
		{
			//if our cover is mor ethan 10 away from the player it is valid
			if ((obstacle.transform.position - player.transform.position).magnitude > keepAwayDist)
			{
				//20% chance to choose any piece of cover
				if (Random.value > 0.2)
				{
					//move a distance of two in a direction the reverse of the direction towards the player
					goal = obstacle.transform.position;
					return (true);
				}
			}
		}
		return (false);
	}

	//Generic Spellcast function to be called by enemy controllers. Takes an array of words to make the spell, a transform representing poistion, and a tranform representing rotation.
	public void SpellCast(string spellstring, Transform rot)
	{
		generator.SpeakSpell(spellstring);
		loadedSpell = generator.MakeSpell();
		GameObject spell = Instantiate(loadedSpell, spellgun.transform.position, rot.rotation) as GameObject;
		SpellShape.SpellShapeType type = spell.GetComponent<SpellShape>().GetShapeType();
		Vector3 distance = Vector3.zero;

		switch (type)
		{
			case SpellShape.SpellShapeType.Ray:
				distance = Vector3.zero;
				spell.transform.parent = spellgun.transform;
				spell.transform.localRotation.eulerAngles.Set(0, 0, 0);
				spell.GetComponent<RayShape>().myLayerMask = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default"));
				spell.SetActive(true);
				break;

			case SpellShape.SpellShapeType.Missile:
				distance = spell.GetComponent<MissileShape>().getRadius() * spellgun.transform.TransformDirection(new Vector3(0, 0, 1));
				spell.GetComponent<Rigidbody>().velocity = spellgun.transform.TransformDirection(new Vector3(0, 0, 10.0f));

				spell.SetActive(true);

				break;
		}

		spell.layer = 14;
		spell.SetActive(true);
	}

	//For dodging the current implementation is a temporary solution to get some basic enemies into the game. The idela solution woul be to calculate an arc that did not contain the player, projectiles, and where the player is pointing (or field of view?) and then move to a random point with that arc.

	//Basic dodge. Randomly dodges in the direction of one of several possible predetermined dodgepoints. 
	//Takes a transform array of dodgepoints, the enemy's navmash agent, and possible values for the speed and acceleration of the dodge. Providing 0 for dodgespeed or doodgeaccel uses the enemies current values.
	//Use for Dumb Enemies
	public void Dodge(Vector3[] dodgepoints, ref UnityEngine.AI.NavMeshAgent agent, float dodgespeed, float dodgeaccel)
	{
		//this is hacky right now. Most enemies will use smart dodge anyways.
		int length = dodgepoints.GetLength(0);
		int i = 0;
		while (i < length)
		{
			if (Random.value > 0.8)
			{
				i++;
				break;
			}
			i++;
		}
		agent.destination = dodgepoints[i - 1];
		if (dodgespeed != 0)
			agent.speed = dodgespeed;
		if (dodgeaccel != 0)
			agent.acceleration = dodgeaccel;
	}

	//SmartDodge for use by wizards and other smart enemies. Takes the same arguments as Dodge(...) in addition to a transform that acts as the object to dodge from.
	public void SmartDodge(Vector3[] dodgepoints, ref UnityEngine.AI.NavMeshAgent agent, float dodgespeed, float dodgeaccel, Transform runaway)
	{
		int length = dodgepoints.GetLength(0);
		int i = 0;
		float best = 0;
		float dist;
		Vector3 bestpos = Vector3.zero;
		//again i'd like to do this smarter. This is a temporary implementation so we can quickly get enemies working.
		while (i < length)
		{
			dist = Vector3.Distance(dodgepoints[i], runaway.position);
			if (dist > best)
			{
				best = dist;
				bestpos = dodgepoints[i];
			}
			i++;
		}
		agent.destination = bestpos;
		if (dodgespeed != 0)
			agent.speed = dodgespeed * timeScale;
		if (dodgeaccel != 0)
			agent.acceleration = dodgeaccel * timeScale;
	}

	//for dodging out of the player's fov.
	public void FOVDodge()
	{
	}

	//Move to a position. Takes a Transform representing the destination and a reference to the Enemy's NavMesh agent. An Overload for inputting the speed and acceleration is provided.
	//Also provides an Overload for taking a Gameobject with a corresponding speed and acceleration overload.
	public void Moveto(Vector3 dest)
	{
		this.agent.destination = dest;
		this.agent.speed = this.movespeed * timeScale;
		this.agent.acceleration = this.moveaccel * timeScale;
	}
	public void Moveto(Vector3 dest, float speed, float accel)
	{
		this.agent.destination = dest;
		this.agent.speed = speed * timeScale;
		this.agent.acceleration = accel * timeScale;
	}
	public void Moveto(Transform dest)
	{
		Moveto(dest.position);
	}
	public void Moveto(Transform dest, float speed, float accel)
	{
		Moveto(dest.position, speed, accel);
	}
	public void Moveto(GameObject dest)
	{
		Moveto(dest.transform);
	}
	public void Moveto(GameObject dest, float speed, float accel)
	{
		Moveto(dest.transform, speed, accel);
	}

	//this serves as an example of how to create GenericBehaivors that utilize the shared attributes of Enemy_Base. A spell could then call the GenericBehaivor.
	//Alternately you could implement a Generic Behaivor and then overload it in the corresponding enemy controller for enemy specific behaivor.
	public void RunAway()
	{
		Vector3 new_dest = agent.destination;
		if (affected_by_fear)
		{
			//if the enemy is affected by fear move in the opposite direction using the enemies run speed.
			new_dest.x *= -1;
			new_dest.z *= -1;
			Moveto(new_dest, runspeed, runaccel);
		}
	}

	protected virtual void Strafe()
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

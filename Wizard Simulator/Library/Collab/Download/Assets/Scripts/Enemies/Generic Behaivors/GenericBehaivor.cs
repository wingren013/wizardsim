using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBehaivor : WizSimBehavior {

    public Transform spellgun;
    public GameObject loadedSpell;
    public GameObject projectile;
    public SpellGenerator generator;
    public Enemy_Base enemy;
    public UnityEngine.AI.NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        this.enemy = gameObject.GetComponent<Enemy_Base>();
        this.agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

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
                distance = spell.GetComponent<Missile>().getRadius() * spellgun.transform.TransformDirection(new Vector3(0, 0, 1));
                spell.GetComponent<Rigidbody>().velocity = spellgun.transform.TransformDirection(new Vector3(0, 0, 10.0f));

                spell.SetActive(true);

                break;
        }

        spell.layer = 14;
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
            agent.speed = dodgespeed;
        if (dodgeaccel != 0)
            agent.acceleration = dodgeaccel;
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
    }
    public void Moveto(Vector3 dest, float speed, float accel)
    {
        this.agent.destination = dest;
        this.agent.speed = speed;
        this.agent.acceleration = accel;
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
    public void RunAway()
    {
        Vector3 new_dest = agent.destination;
        if (enemy.affected_by_fear)
        {
            //if the enemy is affected by fear move in the opposite direction using the enemies run speed.
            new_dest.x *= -1;
            new_dest.z *= -1;
            Moveto(new_dest, enemy.runspeed, enemy.runaccel);
        }
    }
}

using UnityEngine;

/*
 * [SpellEffect Class]
 * 
 * SpellEffects are magical properties that can modify the behavior 
 * of SpellShape game objects and objects that can be affected by magic.
 * 
 * Elemental SpellEffects are normally limited to one per SpellShape
 * 
 */

public abstract class SpellEffect : Spell {
    //Distinguishes the type of spell effect
    public enum SpellEffectType
    {
        Burning,
        Freezing,
        Shining,
        Shocking,
        Time,
        Size,
        Spin,
        Magnetic,
        Repulsive,
        None,
    }

    public SpellEffectType effectType;

    Health healthComponent;
	Enemy_Base enemy;

    // Damaging effects
    //Does it do damage?
    public bool  doesDamage = false;

    //Time of each damage tick
    public float damageFrequency = 0.0f;

    //Time until next damage tick
    public float damageTimer = 0.0f;

    //Damage on contact
    public float damageBurst = 0.0f;

    //Total damage over time
    public float damageOverTime = 0.0f;
    public float damageOverTimeDealt = 0.0f;

    // Denotes that spell should use the collider to transfer itself to other game objects
    public bool isPropagator = false;

    // Denotes number of times propogated effect should also be a propogator
    public int jumpsRemaining = 0;

    public Health.DamageType damageType;

    // Spell Start
    public override void SpellStart()
    {
        healthComponent = GetComponent<Health>();
		enemy = GetComponent<Enemy_Base>();

        //If this effect is attached to a damageable entity, do some burst damage first
        if (doesDamage && healthComponent && damageBurst > 0.0f)
        {
            healthComponent.DoDamage(damageBurst, damageType);
        }
        //If this effect is attached to a spell entity 
        if(GetComponent<SpellShape>() != null)
        {
            permanent = true;
        }
        damageTimer = 1.0f;
        EffectStart();
    }

   //Effects must implement this
    public virtual void EffectStart() { }

    // Effects must implement this
    public virtual void EffectUpdate() { }

    //Sets spell as propogator
    public void SetPropagate(bool isPropagator, int jumps)
    {
        this.isPropagator = isPropagator;
        jumpsRemaining = jumps;
    }

    //Propogate method.  Override this to change how spell propogates.
    public virtual void Propogate(GameObject target)
    {
        if (isPropagator && jumpsRemaining > 0)
        {
            //If it already has this script, refresh it.
            if (target.GetComponent(this.GetType()) != null)
            {
                (target.GetComponent(this.GetType()) as SpellEffect).Refresh();
                return;
            }
            //Otherwise, add a new script to the target.
            SpellEffect propogatedEffect = target.gameObject.AddComponent(this.GetType()) as SpellEffect;

            //Set the propogated effect's propogation mode based on how many jumps are left
            if (jumpsRemaining == 1)
                propogatedEffect.SetPropagate(false, 0);
            else
                propogatedEffect.SetPropagate(true, jumpsRemaining - 1);
            propogatedEffect.magnitude = magnitude;
        }
    }

    //Default damage behavior.  Can be overridden, but should serve most needs

    public virtual void DoBurstDamage()
    {

    }

    public virtual void DoDotDamage()
    {

    }

    //Called by Spell.Update() once per frame
    public override void SpellUpdate()
    {
        if (!permanent && duration <= 0.0f)
        {
            //Do remaining damage for Dots
            if(healthComponent && damageOverTime > 0.0f)
            {
                healthComponent.DoDamage(damageOverTime - damageOverTimeDealt, damageType);
            }
			if (enemy)
			{
				switch (damageType)
				{
					case Health.DamageType.Burn:
						enemy.burning = false;
						break;
					case Health.DamageType.Freeze:
						enemy.frozen = false;
						break;
					case Health.DamageType.Light:
						enemy.illuminated = false;
						break;
				}
			}
            Destroy(this);
        }
        if (doesDamage && damageOverTime > 0.0f)
        {
            damageTimer -= Time.deltaTime;
            if (healthComponent && damageOverTime > 0.0f)
            {
                if (damageTimer <= 0.0f)
                {
                   // Debug.Log(damageOverTime / defaultDuration);

                    damageOverTimeDealt += healthComponent.DoDamage(damageOverTime / defaultDuration, damageType);
                    damageTimer = 1.0f;
                }
            }
			if (enemy)
			{
				switch (damageType)
				{
					case Health.DamageType.Burn:
						enemy.burning = true;
						break;
					case Health.DamageType.Freeze:
						enemy.frozen = true;
						break;
					case Health.DamageType.Light:
						enemy.illuminated = true;
						break;
				}
			}
		}

        EffectUpdate();
    }

    public override void Dispel()
    {

    }
}

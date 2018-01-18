using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 * For any and all objects that can be damaged by magic.
 */

public class Health : WizSimBehavior
{

    public static GameObject damageNumber;

    public List<AudioClip> impactSounds;

    public enum DamageType
    {
        Physical,
        Arcane,
        Burn,
        Freeze,
        Shock,
        Light,
    }

    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float currentHealth;

	public Enemy_Base enemy;

	float resistFire = 1.0f;
    float resistShock = 1.0f;
    float resistFreeze = 1.0f;
    float resistLight = 0.0f;
    float resistPhysical = 1.0f;
    float resistArcane = 0.0f;

    public void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Start()
    {
        if(damageNumber == null)
        {
            damageNumber = Resources.Load("VRDamageNumber") as GameObject;
        }
		enemy = gameObject.GetComponent<Enemy_Base>();
    }


    public float DoDamage(float amount, DamageType type)
    {
        GameObject damagePopup = null;
        float damageDealt = 0.0f;

        switch (type)
        {
            case DamageType.Burn:
                damageDealt = amount * resistFire;
                break;
            case DamageType.Freeze:
                damageDealt = amount * resistFreeze;
                break;
            case DamageType.Shock:
                damageDealt = amount * resistShock;
                break;
            case DamageType.Light:
                damageDealt = amount * resistLight;
                break;
            case DamageType.Physical:
                damageDealt = amount * resistPhysical;
                break;
            case DamageType.Arcane:
                damageDealt = amount * resistArcane;
                break;
        }

        currentHealth -= damageDealt;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth <= 0)
        {
            if (!enemy)
                Destroy(this.gameObject);
            else
                enemy.Die();
        }

        if (impactSounds.Count > 0)
            this.GetComponent<AudioSource>().PlayOneShot(impactSounds[0], 1.0f);

        damagePopup = Instantiate(damageNumber, this.transform.position, Quaternion.identity);
        damagePopup.GetComponent<DamageTextAnimation>().SetDamageText((int)damageDealt);

        BroadcastMessage("Flash");

        return damageDealt;
    }

    public float DoDamageLocation(float amount, DamageType type, Vector3 location)
    {
        GameObject damagePopup = null;
        float damageDealt = 0.0f;

        switch(type)
        {
            case DamageType.Burn:
                damageDealt = amount * resistFire;
                break;
            case DamageType.Freeze:
                damageDealt = amount * resistFreeze;
                break;
            case DamageType.Shock:
                damageDealt = amount * resistShock;
                break;
            case DamageType.Light:
                damageDealt = amount * resistLight;
                break;
            case DamageType.Physical:
                damageDealt = amount * resistPhysical;
                break;
            case DamageType.Arcane:
                damageDealt = amount * resistArcane;
                break;
        }

        currentHealth -= damageDealt;

		if (currentHealth > maxHealth)
			currentHealth = maxHealth;

		if (currentHealth <= 0)
		{
			if (!enemy)
				Destroy(this.gameObject);
			else
				enemy.Die();
		}

        if(impactSounds.Count > 0)
        this.GetComponent<AudioSource>().PlayOneShot(impactSounds[0], 1.0f);

        damagePopup = Instantiate(damageNumber, location, Quaternion.identity);
        damagePopup.GetComponent<DamageTextAnimation>().SetDamageText((int)damageDealt);

        BroadcastMessage("Flash");

        return damageDealt;
    }
}

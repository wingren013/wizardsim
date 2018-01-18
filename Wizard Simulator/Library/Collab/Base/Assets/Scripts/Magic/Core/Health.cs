using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * For any and all objects that can be damaged by magic.
 */

public class Health : MonoBehaviour {

    public static GameObject damageNumber;

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

    float resistFire = 1.0f;
    float resistShock = 1.0f;
    float resistFreeze = 1.0f;
    float resistLight = 0.0f;
    float resistPhysical = 0.0f;
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
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

	public float DoDamage(float amount, DamageType type)
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

        if (currentHealth <= 0)
            Die();

        damagePopup = Instantiate(damageNumber, this.transform.position, Quaternion.identity);
        damagePopup.GetComponent<DamageTextAnimation>().SetDamageText((int)damageDealt);

        SendMessageUpwards("Flash");

        return damageDealt;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticEffect : SpellEffect {

    // Reference to the magnetic effect prefab
    static private GameObject magneticPrefab;
    private GameObject magneticObject;

    // Reference to the spells shape
    private SpellShape.SpellShapeType spellShape;

    // How fast the enemy is pulled towards the position
    public float magneticStrength = 5.0f;

    void Awake() {
        // Assignig test particle effect for debug purposes
        if (magneticPrefab == null)
            magneticPrefab = Resources.Load("Effects/Burning Particles") as GameObject;
        defaultDuration = 5.0f;
    }

    public override void EffectStart() {
        magneticObject = Instantiate(magneticPrefab, this.transform);
        var meshEmmiter = magneticObject.GetComponent<ParticleSystem>().shape;
        // Assigning the shape of the particle emmiter
        meshEmmiter.shapeType = ParticleSystemShapeType.Mesh;
        // checking if the effect has propogated... I think?
        if (this.GetComponentInChildren<MeshFilter>())
            meshEmmiter.mesh = this.GetComponentInChildren<MeshFilter>().mesh;
        magneticObject.SetActive(true);

        // Storing a reference to the shape
        spellShape = GetComponent<SpellShape>().GetShapeType();
    }

    public override void EffectUpdate() {
        Rigidbody enemy = this.GetComponentInChildren<Rigidbody>();

        // Checking if the enemy is in the AOE for this effect
        if (enemy != null) {
            if (spellShape == SpellShape.SpellShapeType.Ray) {
                float speed = magneticStrength * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, this.transform.position, speed);
            } else if (spellShape == SpellShape.SpellShapeType.Bubble) {
                // Not yet implemented
            } else if (spellShape == SpellShape.SpellShapeType.Missile) {
                // Not sure what I should do for this effect for missile
                // Maybe pull everything towards the missle destruction point???
            }
        }
    }

    public override void Dispel() {
        // Destroying the particle emmiter
        Destroy(magneticObject);
    }
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandWizard : MonoBehaviour {
     
    public Transform pos;
    public float spellcooldown = 2.0f;
    private GenericBehaivor BehaviorScript;
    NavMeshAgent agent;
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        if (spellcooldown > 0)
            spellcooldown -= Time.deltaTime;
        else
        {
            BehaviorScript.SpellCast("missile", gameObject.transform);
            spellcooldown = 2.0f;
        }
    }
}

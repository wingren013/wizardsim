﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrandWizard : WizSimBehavior {
     
    public Transform pos;
    public float spellcooldown = 2.0f;
    public GenericBehaivor BehaviorScript;
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
            string test = "missile";
            BehaviorScript.SpellCast(test, this.gameObject.transform);
            spellcooldown = 2.0f;
        }
    }
}

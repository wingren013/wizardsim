using UnityEngine;
using System.Collections;

public class NavControl : WizSimBehavior {
	
	UnityEngine.AI.NavMeshAgent agent;
	bool stopped;
	public Transform target;
	float thinktime = .25f;

	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		
		agent.SetDestination (target.position);
	}
	
	// Update is called once per frame
	void Update () 
	{

			thinktime -= Time.deltaTime;
			if (thinktime < 0) {
					agent.SetDestination (target.position);
			}
	}
}

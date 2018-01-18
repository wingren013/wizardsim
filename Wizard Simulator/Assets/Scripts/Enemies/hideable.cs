using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideable : WizSimBehavior {

	GameObject player;
	RaycastHit hit;
	private bool safe = true;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("VRCamera");
		GameObject.Find("Elementalist").GetComponent<Elementalist>().hidey_holes.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Physics.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, out hit))
		{
			if (hit.collider.gameObject.tag == "player")
			{
				safe = false;
			}
			else
				safe = true;
		}
	}

	public bool isSafe()
	{
		return (safe);
	}
}

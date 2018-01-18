using UnityEngine;
using System.Collections;

public class SpellSwitch : MonoBehaviour {

	public GameObject door;
	public bool activated = false;

	public void FlipSwitch(){
		if(door != null)
		{
			activated = !activated;
			door.SetActive (activated);
		} 
	}

	// Use this for initialization
	void Start () {
		if(door != null)
			door.SetActive (activated);
	}

	void OnCollisionEnter(Collision collision)
	{
		Debug.Log ("Switched");
		if (collision.gameObject.tag == "Spell")
			FlipSwitch ();
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;


public class Spinning : WizSimBehavior
{
	public float offsetx = 0f;
	public float offsety = 0f;
	public float xspeed = 0f;
	public float yspeed = 0f;
	

	void Update ()
	{
		offsetx += xspeed * Time.deltaTime;
		offsety += yspeed * Time.deltaTime;
		gameObject.GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex",new Vector2(offsetx,offsety));
		gameObject.GetComponent<Renderer>().materials[1].SetTextureOffset("_MainTex",new Vector2(-offsetx*.7f+.5f,-offsety*.7f+.5f));
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellChooser : WizSimBehavior {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//gets the angle at which a missile must be launched to arrive at the selected position
	public static float CalcMissileAngle(float target, float speed)
	{
		float g = Physics.gravity.magnitude;
		float x = (speed * speed) + Mathf.Sqrt(Mathf.Pow(speed, 4) - (g * ((g * Mathf.Pow(target, 2)) + (2 * speed * speed))));
		float y = (speed * speed) - Mathf.Sqrt(Mathf.Pow(speed, 4) - (g * ((g * Mathf.Pow(target, 2)) + (2 * speed * speed))));

		x /= g * target;
		y /= g * target;
		x = Mathf.Atan(x);
		y = Mathf.Atan(y);

		if (x != x)
			x = y;
		if (y != y)
			return (0);
		if (y > x)
			return (-y);
		return (-x);
	}

	public static float FindAngle(Transform target, Transform origin)
	{
		Vector3 direction = target.position - origin.position;
		Vector3 horizontal = new Vector3(direction.x, 0, direction.y);
		float angle = Vector3.Angle(direction, horizontal);
		if (direction.y < 0)
			angle *= -1;
		return (angle);
	}

	//static float CheckResistances(Element e) { }

 }

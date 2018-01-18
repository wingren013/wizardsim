using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	public GameObject exit_sphere; //for exiting the game
	public GameObject save_sphere; // for saving
	public GameObject load_sphere; // for loading
	public GameObject control_sphere; //for changing controls
	public GameObject spellword_sphere; // for changing spellwords
	public GameObject leave_sphere; //for leaving the menu
	public List<GameObject> sphere_list;
	public Transform camera_ref;
	// Use this for initializatiossn
	void Start () {
		sphere_list = new List<GameObject>();

		GameObject.Instantiate(exit_sphere);
		sphere_list.Add(exit_sphere);
		exit_sphere.SetActive(false);

		GameObject.Instantiate(save_sphere);
		sphere_list.Add(save_sphere);
		save_sphere.SetActive(false);

		GameObject.Instantiate(load_sphere);
		sphere_list.Add(load_sphere);
		load_sphere.SetActive(false);

		GameObject.Instantiate(control_sphere);
		sphere_list.Add(control_sphere);
		control_sphere.SetActive(false);

		GameObject.Instantiate(spellword_sphere);
		sphere_list.Add(spellword_sphere);
		spellword_sphere.SetActive(false);

		GameObject.Instantiate(leave_sphere);
		sphere_list.Add(leave_sphere);
		leave_sphere.SetActive(false);
	}

	void MenuActivate()
	{
		sphere_list.ForEach(sphere => sphere.SetActive(true));
		//I don't think I care if the scripts aborts when these aren't active but why not?
		if (exit_sphere.activeSelf)
		{
			sphere_list.ForEach(sphere => sphere.transform.position = camera_ref.position);
			//define a ring in front of the camera and then place the spheres at points on those rings.

			float adjust = 0.0f;
			foreach (GameObject sphere in sphere_list)
			{
				float radian = (adjust / sphere_list.Count) * 2 * 3.14f;
				sphere.transform.position = Vector3.Normalize(camera_ref.forward);
				//it complained at me for directly modifying things
				Vector3 okay = sphere.transform.position;
				//THIS MIGHT NEED SOME EDITS
				okay.x *= Mathf.Cos(radian) * 3;
				okay.y *= Mathf.Sin(radian) * 3;
				okay.z *= Mathf.Tan(radian) * 3;
				sphere.transform.position = okay;
				adjust++;
			}
		}
	}

	void MenuDeactivate()
	{
		sphere_list.ForEach(sphere => sphere.SetActive(false));
	}
}

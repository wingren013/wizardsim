using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveGame : MonoBehaviour {

	public string GameDataPath; //path to gamedata directory

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCall()
	{
		StreamReader save_list_file = new StreamReader(GameDataPath + "savelist.dat");
		List<string> saves = new List<string>();
		string s;
		while ((s = save_list_file.ReadLine()) != null)
		{
			saves.Add(s);
		}
		//calls up a radial menu of save slots.
	}
}

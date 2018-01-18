using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class AppState : MonoBehaviour {
	public static AppState instance;
	Process myProcess = null;
	// Use this for initialization
	void Start () 
	{
		if (instance == null) 
		{
			instance = this;
			DontDestroyOnLoad (gameObject);
		} 
		else 
		{
			Destroy (gameObject);
			return;
		}
        SceneManager.LoadScene("levelselect", LoadSceneMode.Single);
    }

	void OnDestroy()
	{
		if(myProcess != null)
			myProcess.Kill ();
	}

	// Update is called once per frame
	void Update () {

	}
	
}



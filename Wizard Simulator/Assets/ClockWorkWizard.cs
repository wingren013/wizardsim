using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ClockWorkWizard : MonoBehaviour {

    public List<AudioClip> voiceclips;
    public float speechtime = 10.0f;
    public float timer = 10.0f;

    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(voiceclips[0],1.0f);
    }

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            GetComponent<AudioSource>().PlayOneShot(voiceclips[Random.Range(1, 6)], 1.0f);
            timer = speechtime;
        }
        
	}
}

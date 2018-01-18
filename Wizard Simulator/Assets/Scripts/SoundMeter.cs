using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundMeter : MonoBehaviour {

	Image meter;
	public float filled = 0.0f;
	// Use this for initialization
	void Start () {
		meter = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		meter.enabled = false;
		meter.fillAmount = filled*3;
		meter.enabled = true;
	}
}

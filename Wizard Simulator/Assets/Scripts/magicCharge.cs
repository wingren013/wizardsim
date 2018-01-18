using UnityEngine;
using System.Collections;

public class magicCharge : MonoBehaviour {
    public MicLevel microphone;
    public ParticleSystem listening;
    public ParticleSystem pulser;
	public ParticleSystem charging_particles;
	public ParticleSystem charging_ball;
	public AudioSource charge_sound;
	public GameObject charged_ball;
	public float original_pitch =  0.1f;
	private float charge_time = 0.0f;
	private float time_to_charge = 0.0f;
	private float pitch_increase = 1.0f; 
	// Use this for initialization
	void Start () {
		charge_sound.pitch = original_pitch;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void StartListen()
    {
        microphone.StartMic();
        listening.gameObject.SetActive(true);
    }

    public void StopListen()
    {
        microphone.StopMic();
        listening.gameObject.SetActive(false);
    }

    public void Listen()
    {
        /*
        var emission = pulser.emission;
        emission.rateOverTime = 10;
        listening.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        */
    }

	public void SetChargeTime(float chargetime)
	{
		charge_time = chargetime;
		charging_particles.playbackSpeed = 1 * (.5f/(chargetime));
		charging_ball.startLifetime = chargetime;
	}

	public void ChargeComplete()
	{
		charged_ball.SetActive (true);
		charging_particles.gameObject.SetActive (false);
		charging_ball.gameObject.SetActive(false);
	}

	public void Charge()
	{
		charged_ball.SetActive (false);
		charging_particles.gameObject.SetActive (true);
		charging_ball.gameObject.SetActive(true);
	}

	public void IncreasePitch ()
	{
		charge_sound.pitch -= Time.deltaTime * pitch_increase;
		//Mathf.Lerp(original_pitch,1.0f,)
	}

	public void Stop()
	{
		charge_sound.pitch = original_pitch;
		charged_ball.SetActive (false);
		charging_particles.gameObject.SetActive (false);
		charging_ball.gameObject.SetActive(false);
	}

	public void ChangeChargeParticle(Material mat)
	{
		charging_particles.GetComponent<ParticleRenderer> ().material = mat;
		charging_ball.GetComponent<ParticleRenderer> ().material = mat;
		charged_ball.GetComponent<ParticleRenderer> ().material = mat;
	}

}

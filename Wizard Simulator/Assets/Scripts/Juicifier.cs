using UnityEngine;
using System.Collections;

public class Juicifier : WizSimBehavior {



    public enum FlashType
    {
        PureWhite,
        Dark
    }
    
    public static Material flashmat;
    Material original;
    public FlashType mode;
    public Renderer renderer;
    bool flashon = false;
    public float intervaltime = .05f;
    float intervaltimer = .05f;
    bool flashing = false;
    bool shaking = false;
    public float flashtime = .5f;
    float flashtimer = .5f;
    public float shaketime = .5f;
    float shaketimer = .5f;
	// Use this for initialization
	void Start () {
		if(flashmat == null) 
            flashmat = Resources.Load ("Flashing") as Material;
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();
        original = renderer.material;
	}
	
	// Update is called once per frame
	void Update () {
		if (flashing)
			DoFlash();
	}

	public void Flash()
	{
		if (!flashing) 
		{
			flashing = true;
		}
		else
		{
			flashtimer = flashtime;
		}
	}

    void DoFlash()
	{
		flashtimer -= Time.deltaTime;
		intervaltimer -= Time.deltaTime;

		if (flashtimer < 0f) 
		{
			flashing = false;
			flashtimer = flashtime;
			FlashOff();
			return;
		}

		if (flashon && intervaltimer < 0f) 
			FlashOff();
		else if (!flashon && intervaltimer < 0f) 
			FlashOn();
	}

	void FlashOn()
	{
		flashon = true;
		intervaltimer = intervaltime;
        if (mode == FlashType.PureWhite)
        {
            renderer.material = flashmat;
            renderer.material.color = Color.white;
        }
        else if (mode == FlashType.Dark)
        {
            renderer.material.color = renderer.material.color / 2.0f;
        }
	}

	void FlashOff()
	{

        flashon = false;
        if (mode == FlashType.PureWhite)
        {

            renderer.material = original;
        }
        else if(mode == FlashType.Dark)
        {
            renderer.material.color = Color.white;
        }
		intervaltimer = intervaltime;
	}



	public void Shake()
	{

	}
}

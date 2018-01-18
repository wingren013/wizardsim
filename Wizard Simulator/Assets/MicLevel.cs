using UnityEngine;

public class MicLevel : MonoBehaviour {

    public AudioSource source;
    public float xspeed = 0.1f;
    public float yspeed = 0.1f;
    private float xoffset = 0.0f;
    private float yoffset = 0.0f;
    public float baselevel;
    public float level;
    public float scalefactor;
    public float decayrate;
    //public Material mat;
    public float[] samples;
    public int sample_start;
    public int sample_end;
    public float average_sample;

    void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[1024];
    }

    public void StartMic()
    {
        /*
        Debug.Log("Mic Starting");
        source.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        source.loop = true;
        while (!(Microphone.GetPosition(null) > 0))
            source.Play();
            */
    }

    public void StopMic()
    {
        /*
        Debug.Log("Mic Stopping");
        source.Stop();
        */
    }



    // Update is called once per frame
    void Update()
    {
        /*
        xoffset += xspeed * Time.deltaTime;
        yoffset += yspeed * Time.deltaTime;

        AudioListener.GetSpectrumData(samples, 0, FFTWindow.Hamming);

        int num_samples = sample_end - sample_start;
        average_sample = 0;
        for (int i = sample_start; i < sample_end; i++)
        {
            //Debug.DrawRay(transform.position + new Vector3(transform.position.x, transform.position.y, transform.position.z + .01f * i), Vector3.up * samples[i] * scalefactor);
            average_sample += samples[i];
        }
        average_sample = average_sample / num_samples;

        if (average_sample * scalefactor > level)
        {
            level = average_sample * scalefactor;
        }
        else
        {
            level = Mathf.Lerp(level, baselevel, Time.deltaTime * decayrate);
            //   level = level - (decayrate * Time.deltaTime);
        }
        */
        //mat.SetFloat("_DistortionScaleX", baselevel + level);
        //mat.SetFloat("_OffsetX", xoffset);
       // mat.SetFloat("_OffsetY", yoffset);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class SpeechHandler : MonoBehaviour {


	public GrammarRecognizer grammarRecognizer;
    private SpellGenerator spellGenerator;
	string[] phrase;
	// Use this for initialization

	void onRecognize(PhraseRecognizedEventArgs args)
	{
        // This may be unstable and require further work.

        if (grammarRecognizer.IsRunning == false)
        {
            Debug.Log("Phrase recognized, aborting.");
            return;
        }
        else
        {
            Debug.Log("onRecognize:" + args.text);
            spellGenerator.SpeakSpell(args.text);
        }
	}

	void Start () {
        spellGenerator = GameObject.Find("Spell Generator").GetComponent<SpellGenerator>();
        grammarRecognizer = new GrammarRecognizer (Application.dataPath + "/Grammar/grammar.xml");
        grammarRecognizer.OnPhraseRecognized += onRecognize;
        PhraseRecognitionSystem.Restart();
        PhraseRecognitionSystem.OnStatusChanged += RecoStatusChanged;
    }

    public void StartListening()
    {
        grammarRecognizer.Start();
    }

    public void StopListening()
    {
        PhraseRecognitionSystem.Restart();
        grammarRecognizer.Stop();
    }

    public void RecoStatusChanged(SpeechSystemStatus status)
    {
        Debug.Log(status);
    }
	
	// Update is called once per frame
	void Update () 
	{
	}
}



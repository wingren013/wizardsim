using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DictationControl : MonoBehaviour {

    private DictationRecognizer Scribe;
    private string result;
    // Use this for initialization
    void Start () {
        Scribe = new DictationRecognizer(ConfidenceLevel.Medium);
        Scribe.AutoSilenceTimeoutSeconds = 1.5f;
        Scribe.InitialSilenceTimeoutSeconds = 2.0f;
        Scribe.DictationComplete += OnDictationComplete;
        Scribe.DictationHypothesis += OnDictationHypothesis;
        Scribe.Start();
    }
	
	// Update is called once per frame
	void Update () {
	}

    void OnDictationComplete(DictationCompletionCause cause)
    {
        if (cause != DictationCompletionCause.Complete)
        {
            Debug.LogErrorFormat("Dictation Failure");
            Debug.LogFormat(result);
        }
    }

    void OnDictationHypothesis(string text)
    {
        result += text;
    }

    public string dictationresult()
    {
        return (result);
    }
}

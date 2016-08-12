using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

[RequireComponent(typeof(KeywordManager))]

public class VoiceManager : Singleton<VoiceManager> {

    private KeywordManager keywordManager;

        
    // Use this for initialization
    void Awake () {
        keywordManager = GetComponent<KeywordManager>();

  

    }

    // Update is called once per frame
    void Update () {
	
	}

    // Update is called once per frame
    public void StartKeyWordManager()
    {
        keywordManager.StartKeywordRecognizer();
    }
    // Update is called once per frame
    public void StopKeyWordManager()
    {
        keywordManager.StopKeywordRecognizer();
    }
    public void CollectEgg()
    {
        if (GestureManager.Instance.FocusedObject != null) { 
            if (GestureManager.Instance.FocusedObject.CompareTag(EggCollectionManager.Instance.EggTag))
            {
                GestureManager.Instance.FocusedObject.SendMessage("OnSelect");

            }
        }
    }

    public void ResetGame()
    {
        //EggCollectionManager.Instance.OnReset();
        PlaySpaceManager.Instance.CreatePlaySpace();
    }

    public void XRayOn()
    {
        GazeManager.Instance.XRayVisionOn = true;
    }

    public void XRayOff()
    {
        GazeManager.Instance.XRayVisionOn = false;
    }
    public void PointerOn()
    {

        global::DirectionIndicator.Instance.DirectionIndicatorHelpOn = true;
     }
    public void PointerOff()
    {
        global::DirectionIndicator.Instance.DirectionIndicatorHelpOn = false;
    }

    public void HelpOn()
    {

        global::DirectionIndicator.Instance.DirectionIndicatorHelpOn = true;
        GazeManager.Instance.XRayVisionOn = true;

    }

    public void XRayTip()
    {
        keywordManager.StopKeywordRecognizer();
        TextToSpeechManager tsm = GetComponent<TextToSpeechManager>();
        tsm.SpeakText("Say XRay On to turn on XRay vision");
        keywordManager.StartKeywordRecognizer();
    }

    public void DirectionIndicatorOn()
    {
        keywordManager.StopKeywordRecognizer();
        TextToSpeechManager tsm = GetComponent<TextToSpeechManager>();
        tsm.SpeakText("Say Pointer On for a pointer to the eggs");
        keywordManager.StartKeywordRecognizer();
    }
}

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

    public void CollectEgg()
    {
        if (GestureManager.Instance.FocusedObject.CompareTag(EggCollectionManager.Instance.EggTag))
        {
            GestureManager.Instance.FocusedObject.SendMessage("OnSelect");

        }
    }

    public void ResetGame()
    {
        EggCollectionManager.Instance.OnReset();
    }
}

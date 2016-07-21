using UnityEngine;
using System.Collections;

public class EggManager : MonoBehaviour {

    public AudioClip SoundWhenEggIsCollected;
    public AudioClip SoundWhenEggIsGazedAt;
    public float Width = 0.2f;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.loop = false;


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        //Play a sound to denote the egg is being destroyed
        if (SoundWhenEggIsCollected != null)
            audioSource.PlayOneShot(SoundWhenEggIsCollected);

        //Destroy the collected egg
        EggCollectionManager.Instance.CollectEgg(gameObject);

    }

    void OnGazeEnter()
    {
        //Play a sound to denote the egg is being gazed at
        if (SoundWhenEggIsGazedAt != null)
            audioSource.PlayOneShot(SoundWhenEggIsGazedAt);
    }

    void OnGazeLeave()
    {
        //Play a sound to denote the gaze is leaving
        if (SoundWhenEggIsGazedAt != null)
            audioSource.PlayOneShot(SoundWhenEggIsGazedAt);
    }
}

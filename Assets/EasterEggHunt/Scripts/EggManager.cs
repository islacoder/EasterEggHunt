using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
public class EggManager : MonoBehaviour {

    public AudioClip SoundWhenEggIsCollected;
    public AudioClip SoundWhenEggIsGazedAt;
    public float Width = 0.2f;
    private GameObject occlusionObject;
    private AudioSource audioSource;
    private Behaviour haloComponent;

    bool isOccluded;

    private Material occludedMaterial;
    private Material touchedMaterial;

    MeshRenderer mr;

    /// <summary>
    /// Points to raycast to when checking for occlusion.
    /// </summary>
    private Vector3[] checkPoints;
    // Use this for initialization
    void Start () {

        occludedMaterial = Resources.Load("OccludedEgg") as Material;
        touchedMaterial = Resources.Load("EggTouched") as Material;

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.loop = false;
        haloComponent = GetComponent("Halo") as Behaviour;
        haloComponent.enabled = false;
        isOccluded = false;
        occlusionObject = gameObject.transform.GetChild(0).gameObject;
        mr = occlusionObject.GetComponent<MeshRenderer>();
        occlusionObject.SetActive(false);

        // Set the check points to use when testing for occlusion.
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Vector3 extents = filter.mesh.bounds.extents;
        Vector3 center = filter.mesh.bounds.center;
        Vector3 top = new Vector3(center.x, center.y + extents.y, center.z);
        Vector3 left = new Vector3(center.x - extents.x, center.y, center.z);
        Vector3 right = new Vector3(center.x + extents.x, center.y, center.z);
        Vector3 bottom = new Vector3(center.x, center.y - extents.y, center.z);

        checkPoints = new Vector3[] { center, top, left, right, bottom };

    }

    void Update()
    {



            // Check to see if any of the planet's boundary points are occluded.
            for (int i = 0; i < checkPoints.Length; i++)
            {
                // 5.a: Convert the current checkPoint to world coordinates.
                // Call gameObject.transform.TransformPoint(checkPoints[i]).
                // Assign the result to a new Vector3 variable called 'checkPt'.
                Vector3 checkPt = gameObject.transform.TransformPoint(checkPoints[i]);

                // 5.a: Call Vector3.Distance() to calculate the distance
                // between the Main Camera's position and 'checkPt'.
                // Assign the result to a new float variable called 'distance'.
                float distance = Vector3.Distance(Camera.main.transform.position, checkPt);

                // 5.a: Take 'checkPt' and subtract the Main Camera's position from it.
                // Assign the result to a new Vector3 variable called 'direction'.
                Vector3 direction = checkPt - Camera.main.transform.position;

                // Used to indicate if the call to Physics.Raycast() was successful.
                bool raycastHit = false;

                // 5.a: Check if the planet is occluded by a spatial mapping surface.
                // Call Physics.Raycast() with the following arguments:
                // - Pass in the Main Camera's position as the origin.
                // - Pass in 'direction' for the direction.
                // - Pass in 'distance' for the maxDistance.
                // - Pass in SpatialMappingManager.Instance.LayerMask as layerMask.
                // Assign the result to 'raycastHit'.
                raycastHit = Physics.Raycast(Camera.main.transform.position, direction, distance, SpatialMappingManager.Instance.LayerMask);

                if (raycastHit)
                {
                    // 5.a: Our raycast hit a surface, so the planet is occluded.
                    // Set the occlusionObject to active.
                    isOccluded = true;
                    // At least one point is occluded, so break from the loop.

                    break;
                }
                else
                {
                    // 5.a: The Raycast did not hit, so the egg is not occluded.
                    // Deactivate the occlusionObject.
                    isOccluded = false;
                }
            }


        if (GazeManager.Instance.XRayVisionOn)
        {
            if (isOccluded) { 
                occlusionObject.SetActive(true);
                
            }
            else { 
                occlusionObject.SetActive(false);
                

            }


        }
        else
        {
            //X Ray vision is not on so the egg is occluded
            occlusionObject.SetActive(false);

        }
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

        haloComponent.enabled = true;
        if (GazeManager.Instance.XRayVisionOn)
        {
            if (isOccluded)
            {
                mr.material  = touchedMaterial;

            }
        }
    }

    void OnGazeLeave()
    {
        //Play a sound to denote the gaze is leaving
        if (SoundWhenEggIsGazedAt != null)
            audioSource.PlayOneShot(SoundWhenEggIsGazedAt);

        haloComponent.enabled = false;

        if (GazeManager.Instance.XRayVisionOn)
        {
            if (isOccluded)
            {
                mr.material = occludedMaterial;

            }
        }

    }
}

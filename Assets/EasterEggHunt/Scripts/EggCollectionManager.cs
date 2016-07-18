using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using HoloToolkit.Unity;


public class EggCollectionManager : Singleton<EggCollectionManager>
{

    [Tooltip("Egg Prefab")]
    public GameObject Egg;


    private List<GameObject> eggObjects;

    private int eggObjectCount;

    private int collectedEggObjects;

    public TypogenicText scoreText;
    public GameObject scoreTextGameObject;


    /// <summary>
    /// Gets the tag on the Egg object.
    /// </summary>
    public string EggTag
    {
        get { return Egg.tag; }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Generates a collection of Egg objects and sets them on floors and tables
    /// </summary>
    /// <param name="floors"></param>
    /// <param name="tables"></param>
    public void GenerateEggsInWorld(List<GameObject> flats)
    {
        List<GameObject> availablePlanes = new List<GameObject>();

        CapsuleCollider eggCollider = Egg.GetComponent<CapsuleCollider>();

        eggObjects = new List<GameObject>();

        foreach (GameObject surface in flats)
        {
            Collider planeCollider = surface.GetComponent<Collider>();
            if (!(eggCollider.bounds.size.x > planeCollider.bounds.size.x || eggCollider.bounds.size.z > planeCollider.bounds.size.z))
            {
                availablePlanes.Add(surface);
            }

        }

        eggObjectCount = availablePlanes.Count;

        foreach (GameObject surface in availablePlanes)
        {
            SurfacePlane plane = surface.GetComponent<SurfacePlane>();
            Vector3 position;
            //1 position.x = surface.transform.position.x;// + (plane.PlaneThickness * plane.SurfaceNormal);
           //2 position.y = plane.Plane.Bounds.Center.y+ plane.PlaneThickness + 0.1f;
            //3position.z = surface.transform.position.z;

            position = surface.transform.position + ((plane.PlaneThickness + 0.1f) * plane.SurfaceNormal);
            Quaternion rotation = surface.transform.rotation;//Egg.transform.localRotation;
            Rigidbody rb = Egg.GetComponent<Rigidbody>();
            rb.useGravity = false;
            GameObject eggClone = Instantiate(Egg, position, rotation) as GameObject;
            eggObjects.Add(eggClone);

        }
        scoreTextGameObject.SetActive(true);
        scoreText.Text = "You have collected " + collectedEggObjects + " out of " + eggObjectCount;
    }



    /// <summary>
    /// Adjusts the initial position of the object if it is being occluded by the spatial map.
    /// </summary>
    /// <param name="position">Position of object to adjust.</param>
    /// <param name="surfaceNormal">Normal of surface that the object is positioned against.</param>
    /// <returns></returns>
    private Vector3 AdjustPositionWithSpatialMap(Vector3 position, Vector3 surfaceNormal)
    {
        Vector3 newPosition = position;
        RaycastHit hitInfo;
        float distance = 0.5f;

        // Check to see if there is a SpatialMapping mesh occluding the object at its current position.
        if (Physics.Raycast(position, surfaceNormal, out hitInfo, distance, SpatialMappingManager.Instance.LayerMask))
        {
            // If the object is occluded, reset its position.
            newPosition = hitInfo.point;
        }

        return newPosition;
    }

    public void CollectEgg(GameObject egg)
    {
        collectedEggObjects++;
        eggObjects.Remove(egg);
        Destroy(egg, 1.0f);
        if (collectedEggObjects == eggObjectCount)
            scoreText.Text = "Congratulations";
        else
            scoreText.Text = "You have collected " + collectedEggObjects + " out of " + eggObjectCount;



    }

    public void OnReset()
    {


        collectedEggObjects = 0;
        for (int i = 0; i < eggObjects.Count; i++)
        {
            GameObject egg = eggObjects[i];
            Destroy(egg);
        }
        eggObjects.Clear();
        GenerateEggsInWorld(PlaySpaceManager.Instance.flat_surfaces);
        scoreText.Text = "You have collected " + collectedEggObjects + " out of " + eggObjectCount;

    }

    void StartGame()
    {



    }


}

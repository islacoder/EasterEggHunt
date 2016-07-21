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
    
    private string[] colors = { "pink", "blue", "orange", "yellow", "green", "purple" };

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
        List<Vector3> availableEggSlots = new List<Vector3>();
        List<GameObject> availablePlanes = new List<GameObject>();


        CapsuleCollider eggCollider = Egg.GetComponent<CapsuleCollider>();
        EggManager eggManager = Egg.GetComponent<EggManager>();
        
        eggObjects = new List<GameObject>();

        foreach (GameObject surface in flats)
        {
            Collider planeCollider = surface.GetComponent<Collider>();
            if (!(eggManager.Width > planeCollider.bounds.size.x || eggManager.Width > planeCollider.bounds.size.z))
            {
                availablePlanes.Add(surface);
                //SurfacePlane plane = surface.GetComponent<SurfacePlane>();

            }
        }

        if (availablePlanes.Count == 0)
        {
            PlaySpaceManager.Instance.ScanSpace();
        }

        //    if (!(eggManager.Width > planeCollider.bounds.size.x || eggManager.Width > planeCollider.bounds.size.z))
        //    {
        //        availablePlanes.Add(surface);
        //        SurfacePlane plane = surface.GetComponent<SurfacePlane>();

        //        Vector3 point0 = new Vector3(planeCollider.bounds.min.x, point3.y, planeCollider.bounds.min.z);
        //        Vector3 point1 = new Vector3(point0.x, point3.y, point3.z);
        //        Vector3 point2 = new Vector3(point3.x, point3.y, point0.z);
        //        for (Vector3 point = point0; point.x < point3.x; point = Vector3.MoveTowards(point, point2, eggManager.Width))
        //            for (Vector3 pointa = point; pointa.z < point3.z; pointa = Vector3.MoveTowards(pointa, point1, eggManager.Width))
        //            {
        //                // Vector3 pos = new Vector3(x + eggManager.Width / 2.0f, planeCollider.bounds.max.y, z + +eggManager.Width / 2.0f) + ((plane.PlaneThickness + 0.01f) * plane.SurfaceNormal);
        //                availableEggSlots.Add(pointa);

        //            }


        //    }

        //}

        //eggObjectCount = Random.Range(1, 6);

        //for (int i = 0; i< availableEggSlots.Count; i++)
        //{
        //    //int randomSlot = Random.Range(0, availableEggSlots.Count - 1);
        //    int randomSlot = i;
        //    Vector3 position = availableEggSlots[randomSlot];
        //    availableEggSlots.RemoveAt(i);
        //    Quaternion rotation = Egg.transform.localRotation;
        //    Rigidbody rb = Egg.GetComponent<Rigidbody>();
        //    rb.useGravity = false;
        //    GameObject eggClone = Instantiate(Egg, position, rotation) as GameObject;
        //    eggObjects.Add(eggClone);

        //}
        Random.InitState(System.DateTime.Now.Millisecond);
        eggObjectCount = Random.Range(1, availablePlanes.Count);


        for (int i = 0; i < eggObjectCount; i++)
        //foreach (GameObject surface in availablePlanes)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            int selectedPlane = Random.Range(0, availablePlanes.Count - 1);
            GameObject surface = availablePlanes[selectedPlane];
            availablePlanes.RemoveAt(selectedPlane);
            SurfacePlane plane = surface.GetComponent<SurfacePlane>();
            Vector3 position;

            position = surface.transform.position + ((plane.PlaneThickness + 0.03687834f) * new Vector3(0f,1f,0f));
            Quaternion rotation = Egg.transform.localRotation;
            Rigidbody rb = Egg.GetComponent<Rigidbody>();
            rb.useGravity = false;
            Random.InitState(System.DateTime.Now.Millisecond);

            string color = colors[Random.Range(0, colors.Length - 1)];
            GameObject eggClone = Instantiate(Egg, position, rotation) as GameObject;
            Renderer rend = eggClone.GetComponent<Renderer>();
            Texture texture = Resources.Load(color) as Texture;
            rend.material.mainTexture = texture;
            eggObjects.Add(eggClone);

        }
        scoreTextGameObject.SetActive(true);
        scoreText.Text = "You have " + eggObjectCount + (eggObjectCount==1?" egg":" eggs") +  " to collect.";
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
            scoreText.Text = "Congratulations! You have collected all eggs.";
        else
            scoreText.Text = "Score: " + collectedEggObjects + "/" + eggObjectCount;



    }

    public void OnReset()
    {

        GameObject g = GameObject.FindGameObjectWithTag("ScanningTextBox");
        g.SetActive(true);

        collectedEggObjects = 0;
        for (int i = 0; i < eggObjects.Count; i++)
        {
            GameObject egg = eggObjects[i];
            Destroy(egg);
        }
        eggObjects.Clear();
        scoreTextGameObject.SetActive(false);
        PlaySpaceManager.Instance.ScanSpace();
        //GenerateEggsInWorld(PlaySpaceManager.Instance.flat_surfaces);
        //scoreText.Text = "You have collected " + collectedEggObjects + " out of " + eggObjectCount;

    }

    void StartGame()
    {



    }




}

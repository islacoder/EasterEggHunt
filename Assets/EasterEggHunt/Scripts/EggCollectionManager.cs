using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using HoloToolkit.Unity;


public class EggCollectionManager : Singleton<EggCollectionManager>
{

    [Tooltip("Egg Prefab")]
    public GameObject Egg;


    private List<GameObject> eggObjects = new List<GameObject>();

    private int eggObjectCount;

    private int collectedEggObjects;

    private float EggGameStarted;
    private float timeToWaitForDirectionMessage = 20f;

    public TypogenicText scoreText;
    public GameObject scoreTextGameObject;

    private bool hasGameStarted = false;
    
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
        hasGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasGameStarted)
            return;
        if (DirectionIndicator.Instance.DirectionIndicatorHelpOn)
            return;
        if (Time.time - EggGameStarted > timeToWaitForDirectionMessage)
        {
            EggGameStarted = Time.time;
            timeToWaitForDirectionMessage = 2 * timeToWaitForDirectionMessage;

            VoiceManager.Instance.DirectionIndicatorOn();
        }
    }



    public void GenerateEggsInWorld(List<GameObject> flats, float floor_height)
    {
        List<Vector3> availableEggSlots = new List<Vector3>();
        List<GameObject> availablePlanes = new List<GameObject>();


        CapsuleCollider eggCollider = Egg.GetComponent<CapsuleCollider>();


        List<Vector3> potentialEggPositions = GetEggPositions(flats, new Vector3(.3f, .15f, .3f),floor_height);

        if (potentialEggPositions.Count == 0)
        {
            //DING DONG NO positions
            scoreTextGameObject.SetActive(false);
            scoreText.Text = "";
            GameObject g = GameObject.FindGameObjectWithTag("ScanningTextBox");
            MeshRenderer mr = g.GetComponent<MeshRenderer>();
            mr.enabled = true;
            PlaySpaceManager.Instance.StatusText.Text = "I haven't found been able to scan your space.  Say 'Reset Game' if you want to start again";

            return;
        }



        Random.InitState(System.DateTime.Now.Millisecond);
        if (potentialEggPositions.Count == 1)
            eggObjectCount = 1;
        else if (potentialEggPositions.Count < 6)
            eggObjectCount = Random.Range(2, potentialEggPositions.Count);
        else eggObjectCount = Random.Range(5, 11);

        //eggObjectCount = potentialEggPositions.Count;

        for (int i = 0; i < eggObjectCount; i++)
        //foreach (GameObject surface in availablePlanes)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            int selectedPos = Random.Range(0, potentialEggPositions.Count);


            Vector3 position = potentialEggPositions[selectedPos] + ((.07f) * new Vector3(0f, 1f, 0f));
            potentialEggPositions.RemoveAt(selectedPos);


            Quaternion rotation = Egg.transform.localRotation;
            Rigidbody rb = Egg.GetComponent<Rigidbody>();
            rb.useGravity = false;
            Random.InitState(System.DateTime.Now.Millisecond);

            string color = colors[Random.Range(0, colors.Length)];
            GameObject eggClone = Instantiate(Egg, position, rotation) as GameObject;
            Renderer rend = eggClone.GetComponent<Renderer>();
            Texture texture = Resources.Load(color) as Texture;
            rend.material.mainTexture = texture;
            eggObjects.Add(eggClone);

        }
        scoreTextGameObject.SetActive(true);
        scoreText.Text = "You have " + eggObjectCount + (eggObjectCount == 1 ? " egg" : " eggs") + " to collect.";
        VoiceManager.Instance.XRayTip();
        EggGameStarted = Time.time;
        hasGameStarted = true;
        VoiceManager.Instance.StartKeyWordManager();

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
        EggGameStarted = Time.time;
        timeToWaitForDirectionMessage = 2 * timeToWaitForDirectionMessage;  
        eggObjects.Remove(egg);
        Destroy(egg, .5f);
        if (collectedEggObjects == eggObjectCount)
        {
            scoreText.Text = "Congratulations!\r\nYou have collected all eggs.";
            hasGameStarted = false;
            //VoiceManager.Instance.StopKeyWordManager();

        }
        else
            scoreText.Text = "Score: " + collectedEggObjects + "/" + eggObjectCount;



    }

    public void Init()
    {

        GameObject g = GameObject.FindGameObjectWithTag("ScanningTextBox");
        MeshRenderer mr = g.GetComponent<MeshRenderer>();
        mr.enabled = true;

        collectedEggObjects = 0;
        for (int i = 0; i < eggObjects.Count; i++)
        {
            GameObject egg = eggObjects[i];
            Destroy(egg);
        }
        eggObjects.Clear();
        scoreTextGameObject.SetActive(false);

        hasGameStarted = false;
    }



    List<Vector3> GetEggPositions(List<GameObject> planes,  Vector3 minSize, float floor_height)

    {
        List<Vector3> positions = new List<Vector3>();

        foreach (GameObject plane in planes) {

            if (plane.transform.position.y > 0.6f) continue;
            if (plane.transform.position.y < floor_height) continue;
            Vector3 planeSize = plane.transform.localScale;

            if (minSize.x > planeSize.x || minSize.z > planeSize.y) continue;

            int gridsX = (int)(planeSize.x / (minSize.x));
            int gridsZ = (int)(planeSize.y / (minSize.z));

            float gridSizeX = 0, gridSizeZ = 0;

            if (gridsX > 0)
                gridSizeX = 1.0f / gridsX;

            if (gridsZ > 0)
                gridSizeZ = 1.0f / gridsZ;



            for (int x = 0; x < gridsX; x++)
            {
                for (int z = 0; z < gridsZ; z++)
                {
                    float localX = -0.5f + gridSizeX / 2f + x * gridSizeX;
                    float localZ = -0.5f + gridSizeZ / 2f + z * gridSizeZ;
                    Vector3 pos = plane.transform.TransformPoint(new Vector3(localX, localZ, minSize.y / 2f));
                    Vector3 camera_position = Camera.main.transform.position;
                    float distance = Vector3.Distance(camera_position, pos);

                    // 5.a: Take 'checkPt' and subtract the Main Camera's position from it.
                    // Assign the result to a new Vector3 variable called 'direction'.
                    Vector3 direction =  camera_position - pos;

                    // Used to indicate if the call to Physics.Raycast() was successful.
                    bool raycastHit = false;

                    // 5.a: Check if the planet is occluded by a spatial mapping surface.
                    // Call Physics.Raycast() with the following arguments:
                    // - Pass in the Main Camera's position as the origin.
                    // - Pass in 'direction' for the direction.
                    // - Pass in 'distance' for the maxDistance.
                    // - Pass in SpatialMappingManager.Instance.LayerMask as layerMask.
                    // Assign the result to 'raycastHit'.
                    raycastHit = Physics.Raycast(camera_position, direction, distance, SpatialMappingManager.Instance.LayerMask);

                    if (distance < 6f  )
                        positions.Add(pos); 
                }
            }
        }

        return positions;
    }


    public GameObject FindClosestObject(Vector3 fromPoint)
    {
        float dist = 99999;
        GameObject closestEgg = null;
        foreach (GameObject egg in eggObjects)
        {
            float distance = Vector3.Distance(egg.transform.position, fromPoint);
            if (distance < dist)
            {
                dist = distance;
                closestEgg = egg;
            }
        }
        return closestEgg;
    }
}

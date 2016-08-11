using System.Collections.Generic;
using System;
using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// The PlaySpaceManager class allows applications to scan the environment for a specified amount of time 
/// and then process the Spatial Mapping Mesh (find planes, remove vertices) after that time has expired.
/// </summary>
public class PlaySpaceManager : Singleton<PlaySpaceManager>
{
    [Tooltip("How much time (in seconds) that the SurfaceObserver will run after being started; used when 'Limit Scanning By Time' is checked.")]
    public float scanTime = 10.0f;

    [Tooltip("Material to use when rendering Spatial Mapping meshes while the observer is running.")]
    public Material defaultMaterial;

    [Tooltip("Optional Material to use when rendering Spatial Mapping meshes after the observer has been stopped.")]
    public Material secondaryMaterial;

    [Tooltip("Minimum number of floor planes required in order to exit scanning/processing mode.")]
    public uint minimumFloors = 1;

    [Tooltip("TextBox for Status")]
    public TypogenicText StatusText ;

    private AudioSource audioSource;

    public AudioClip waitingClip;

    public List<GameObject> flat_surfaces { get; private set; }
    public float floor_height { get; private set; }

    /// <summary>
    /// Indicates if processing of the surface meshes is complete.
    /// </summary>
    private bool meshesProcessed = false;

    private int timeInSeconds;

    private float lastFrameTime;



    /// <summary>
    /// GameObject initialization.
    /// </summary>
    private void Start()
    {
        // Register for the MakePlanesComplete event.
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        CreatePlaySpace();

    }

    public void CreatePlaySpace()
    {

        VoiceManager.Instance.StopKeyWordManager();
        global::DirectionIndicator.Instance.DirectionIndicatorHelpOn = false;
        GazeManager.Instance.XRayVisionOn = false;

        EggCollectionManager.Instance.Init();
        // Update surfaceObserver and storedMeshes to use the same material during scanning.
        SpatialMappingManager.Instance.SetSurfaceMaterial(defaultMaterial);

        meshesProcessed = false;


        timeInSeconds = (int)scanTime;
        StatusText.Text = "Welcome to HoloEgg Hunt\r\n";


        if (!SpatialMappingManager.Instance.IsObserverRunning())
        {
            // Start the observer.
            SpatialMappingManager.Instance.StartObserver();
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = waitingClip;
        audioSource.loop = true;
        audioSource.volume = .5f;
        audioSource.Play();
    }


    /// <summary>
    /// Called once per frame.
    /// </summary>
    private void Update()
    {
        // Check to see if the spatial mapping data has been processed yet.
        if (!meshesProcessed)
        {
            // Check to see if enough scanning time has passed
            // since starting the observer.
            if ((Time.time - SpatialMappingManager.Instance.StartTime) < scanTime)
            {
                // If we have a limited scanning time, then we should wait until
                // enough time has passed before processing the mesh.

                if (Time.time > 1f + lastFrameTime)
                {
                    timeInSeconds--;
                    lastFrameTime = Time.time;
                    StatusText.Text = "Look around to scan your space ...\r\n " + timeInSeconds + " seconds";

                }


            }
            else
            {
                // The user should be done scanning their environment,
                // so start processing the spatial mapping data...


                if (SpatialMappingManager.Instance.IsObserverRunning())
                {
                    // Stop the observer.
                    SpatialMappingManager.Instance.StopObserver();
                }

                StatusText.Text = "";

                // Call CreatePlanes() to generate planes.
                CreatePlanes();


                // Set meshesProcessed to true.
                meshesProcessed = true;
            }
        }
    }

    /// <summary>
    /// Handler for the SurfaceMeshesToPlanes MakePlanesComplete event.
    /// </summary>
    /// <param name="source">Source of the event.</param>
    /// <param name="args">Args for the event.</param>
    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        // Collection of floor planes that we can use to set horizontal items on.
        flat_surfaces = new List<GameObject>();
        flat_surfaces = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor|PlaneTypes.Table);
        List<GameObject> floor_surfaces = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor);
        floor_height = -2f;
        if (floor_surfaces.Count > 0)
            floor_height = floor_surfaces[0].transform.position.y;
         // Check to see if we have enough flat surfaces to start processing.
        if (flat_surfaces.Count >= 1)
        {
            // Reduce our triangle count by removing any triangles
            // from SpatialMapping meshes that intersect with active planes.
            RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);

            // After scanning is over, switch to the secondary (occlusion) material.
            SpatialMappingManager.Instance.SetSurfaceMaterial(secondaryMaterial);
            StatusText.Text = "Game Started";

            Debug.Log("Finished making flat surfaces.");
            audioSource.Stop();

            GameObject g = GameObject.FindGameObjectWithTag("ScanningTextBox");
            MeshRenderer mr = g.GetComponent<MeshRenderer>();
            mr.enabled = false;
            EggCollectionManager.Instance.GenerateEggsInWorld(flat_surfaces, floor_height);
            //StatusText.gameObject


        }
        else
        {

            //ScanSpace();
            StatusText.Text = "I haven't found been able to scan your space.  Say 'Reset game' if you want to start again";

        }
    }

    /// <summary>
    /// Creates planes from the spatial mapping surfaces.
    /// </summary>
    private void CreatePlanes()
    {
        // Generate planes based on the spatial map.
        SurfaceMeshesToPlanes surfaceToPlanes = SurfaceMeshesToPlanes.Instance;
        if (surfaceToPlanes != null && surfaceToPlanes.enabled)
        {
            surfaceToPlanes.MakePlanes();
        }
    }


    /// <summary>
    /// Removes triangles from the spatial mapping surfaces.
    /// </summary>
    /// <param name="boundingObjects"></param>
    private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
    {
        RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
        if (removeVerts != null && removeVerts.enabled)
        {
            removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
        }
    }

    /// <summary>
    /// Called when the GameObject is unloaded.
    /// </summary>
    private void OnDestroy()
    {
        if (SurfaceMeshesToPlanes.Instance != null)
        {
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;
        }
    }


}
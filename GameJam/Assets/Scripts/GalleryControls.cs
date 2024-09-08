using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class GalleryControls : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput playerInput;

    [Header("Camera Variables")]
    [Tooltip("Uses normal indexes. First painting is at index 1, second painting is at index 2, etc.")]
    [SerializeField] private int startingPaintingIndex;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraAccelerationSpeed;
    [SerializeField] private GameObject[] paintingPositions;
    [SerializeField] private GameObject[] altPaintingPositions;

    GameManager gameManager;
    AudioManager audioManager;

    private InputAction moveCamera;
    private InputAction switchBetweenAlts;

    private bool isCameraMoving;
    private bool isCameraMovingBetweenAlts;
    private bool isCamInMotion;
    private bool isCamInAltPaintings;
    private int currentPaintingIndex;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Gets game manager
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;

        //Binds actions to keys
        playerInput.currentActionMap.Enable();
        moveCamera = playerInput.currentActionMap.FindAction("MoveCamera");
        switchBetweenAlts = playerInput.currentActionMap.FindAction("SwitchPaintingInGallery");

        //Binds actions to methods
        moveCamera.started += MoveCamera_Started;
        switchBetweenAlts.started += SwitchBetweenAlts_Started;

        //Initializes Camera variables
        isCameraMoving = false;
        isCamInMotion = false;
        isCamInAltPaintings = false;
        isCameraMovingBetweenAlts = false;

        currentPaintingIndex = startingPaintingIndex - 1;
        mainCamera.transform.position = paintingPositions[currentPaintingIndex].transform.position;
    }

    public void FixedUpdate()
    {
        if (!FindObjectOfType<GameManager>().CamIsShaking && !FindObjectOfType<PaintingManager>().PaintingCameraOverride)
        {
            HandleCameraMovement();
        }
    }

    private void OnDisable()
    {
        moveCamera.started -= MoveCamera_Started;
    }

    /// <summary>
    /// Handles the camera movement via specified inputs
    /// </summary>
    private void HandleCameraMovement()
    {
        //If camera move input has been detected
        if (isCameraMoving)
        {
            //Set to false
            isCameraMoving = false;
            isCamInMotion = true;

            //Retreives Input

            //If Move Camera Right input is detected and camera is not on right most painting, increase painting index
            if (moveCamera.ReadValue<float>() > 0 && currentPaintingIndex < paintingPositions.Length - 1)
            {
                if (!isCamInAltPaintings || (isCamInAltPaintings && currentPaintingIndex < 3))
                    currentPaintingIndex++;
            }

            //If Move Camera Left input is detected and camera is not on left most painting, decrease painting index
            else if (moveCamera.ReadValue<float>() < 0 && currentPaintingIndex > 0)
            {
                currentPaintingIndex--;
            }
        }

        if (isCameraMovingBetweenAlts)
        {
            if (currentPaintingIndex < 4)
            {
                isCameraMovingBetweenAlts = false;
                isCamInMotion = true;

                if (switchBetweenAlts.ReadValue<float>() > 0 && isCamInAltPaintings)
                {
                    isCamInAltPaintings = false;
                }
                else if (switchBetweenAlts.ReadValue<float>() < 0 && !isCamInAltPaintings)
                {
                    isCamInAltPaintings = true;
                }
            }
        }

        //Plays Painting Move Audio
        audioManager.Play("Switch Paintings");

        if (!isCamInAltPaintings)
        {
            //Moves camera to current painting index
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
                paintingPositions[currentPaintingIndex].transform.position, cameraAccelerationSpeed * Time.deltaTime * gameManager.expectedFrameRate);


            if (mainCamera.transform.position == paintingPositions[currentPaintingIndex].transform.position)
                isCamInMotion = false;
        }
        else
        {
            //Moves camera to current painting index
            if (currentPaintingIndex < 4)
            {
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
                    altPaintingPositions[currentPaintingIndex].transform.position, cameraAccelerationSpeed * Time.deltaTime * gameManager.expectedFrameRate);


                if (mainCamera.transform.position == altPaintingPositions[currentPaintingIndex].transform.position)
                    isCamInMotion = false;
            }
        }
        
    }

    public void CameraMovementOverride(int index)
    {
        currentPaintingIndex = index;
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
           paintingPositions[index].transform.position, cameraAccelerationSpeed * Time.deltaTime * gameManager.expectedFrameRate);
    }

    /// <summary>
    /// If move camera input is detected, this method is called automatically
    /// </summary>
    private void MoveCamera_Started(InputAction.CallbackContext obj)
    {
        isCameraMoving = true;
    }

    private void SwitchBetweenAlts_Started(InputAction.CallbackContext obj)
    {
        isCameraMovingBetweenAlts = true;
    }

    /// <summary>
    /// Creates Gizmos for debugging
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //Gets width and height of the camera
        float height = 2 * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        //Creates an outline of the range the camera can see at each position from paintingDocks
        int arrayLength = paintingPositions.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            Gizmos.DrawWireCube(paintingPositions[i].transform.position, new Vector2(width, height));
        }

        Gizmos.color = Color.red;
        //Creates an outline of the range the camera can see at each position from alt painting docks
        int altArrayLength = altPaintingPositions.Length;
        for (int i = 0; i < altArrayLength; i++)
        {
            Gizmos.DrawWireCube(altPaintingPositions[i].transform.position, new Vector2(width, height));
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInputBehavior : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput playerInput;
    InputAction pause;
    InputAction click;

    Vector2 mPosVector;
    [SerializeField] private Sprite tf2Coconut;

    [Header("Flashlight Variable")]
    [SerializeField] private Transform lightObject;
    [SerializeField] private float maxMouseSpeed;

    [Header("Camera Variables")]
    [Tooltip("Uses normal indexes. First painting is at index 1, second painting is at index 2, etc.")]
    [SerializeField] private int startingPaintingIndex;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraAccelerationSpeed;
    [SerializeField] private GameObject[] paintingPositions;

    GameManager gameManager;
    private InputAction moveCamera;

    //These affect nothing. Leave them alone. 
    private InputAction nothingToSeeHere;

    private Vector2 mousePosition;
    private float fadeTimeThatAffectsNothing = .3f;
    private int numFadeStepsThatAffectsNothing = 10;
    private float transparencyThatAffectsNothing = .15f;
    private float timerThatAffectsNothing = 3;
    private bool isPaused = false;
    private bool isCameraMoving;
    private int currentPaintingIndex;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Gets game manager
        gameManager = GameManager.Instance;

        //Binds actions to keys
        playerInput.currentActionMap.Enable();
        pause = playerInput.currentActionMap.FindAction("Pause");
        nothingToSeeHere = playerInput.currentActionMap.FindAction("NothingToSee");
        click = playerInput.currentActionMap.FindAction("Click");
        moveCamera = playerInput.currentActionMap.FindAction("MoveCamera");

        //Binds actions to methods
        pause.performed += Pause_performed;
        click.performed += Click_performed;
        moveCamera.started += MoveCamera_Started;

        nothingToSeeHere.performed += contx => StartCoroutine(NothingToSeeHere_performed());

        //Initializes Camera variables
        isCameraMoving = false;
        currentPaintingIndex = startingPaintingIndex - 1;
        mainCamera.transform.position = paintingPositions[currentPaintingIndex].transform.position;
    }

    public void FixedUpdate()
    {
        HandleLightMovement();
        HandleCameraMovement();
        
    }

    private void OnDisable()
    {
        pause.performed -= Pause_performed;
        nothingToSeeHere.performed -= contx => StartCoroutine(NothingToSeeHere_performed());
        click.performed -= Click_performed;
    }

    /// <summary>
    /// Handles the movement of the light source via the mouse movements
    /// </summary>
    private void HandleLightMovement()
    {
        //Gets mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        //Moves light to mouse position
        lightObject.position = Vector2.MoveTowards(lightObject.position, mousePosition, maxMouseSpeed);
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

            //Retreives Input
            
            //If Move Camera Right input is detected and camera is not on right most painting, increase painting index
            if (moveCamera.ReadValue<float>() > 0 && currentPaintingIndex < paintingPositions.Length - 1)
            {
                currentPaintingIndex++;
            }

            //If Move Camera Left input is detected and camera is not on left most painting, decrease painting index
            else if (moveCamera.ReadValue<float>() < 0 && currentPaintingIndex > 0)
            {
                currentPaintingIndex--;
            }
        }

        //Moves camera to current painting index
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
            paintingPositions[currentPaintingIndex].transform.position, cameraAccelerationSpeed * Time.deltaTime * gameManager.expectedFrameRate);
    }

    private void Click_performed(InputAction.CallbackContext obj)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.zero);
        if(hit.collider != null)
        {
            print(hit.transform.gameObject.name);
            if(hit.transform.gameObject.tag == "Apparation")
            {
                Apparation aRef = FindObjectOfType<PaintingManager>().RetrieveApparationInstance(hit.transform.gameObject.name);
                if(aRef!=null && aRef.IsApparating && !aRef.HasBeenCaught)
                {
                    StopCoroutine(aRef.StartApparation());
                    aRef.Caught();
                    Destroy(aRef.Sr.gameObject);   //Returns apparation to normal
                    FindObjectOfType<GameManager>().IncreaseScore();
                    //Stab animation
                    //Goop
                }
                else if (aRef!=null && aRef.HasApparated)
                {
                    //Lose points
                }
            }
        }
        //TODO
    }

    /// <summary>
    /// Leave this function alone. I swear it's required
    /// </summary>
    /// <param name="obj"></param>
    private IEnumerator NothingToSeeHere_performed()
    {
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = tf2Coconut;
        for (int i=0; i < numFadeStepsThatAffectsNothing; i++)
        {
            sr.color = new Color(1, 1, 1, transparencyThatAffectsNothing / (numFadeStepsThatAffectsNothing - i));
            yield return new WaitForSeconds(fadeTimeThatAffectsNothing / numFadeStepsThatAffectsNothing);
        }
        yield return new WaitForSeconds(timerThatAffectsNothing - 2*(fadeTimeThatAffectsNothing));
        for (int i = numFadeStepsThatAffectsNothing-1; i >=0; i--)
        {
            sr.color = new Color(1, 1, 1, transparencyThatAffectsNothing / (numFadeStepsThatAffectsNothing - i));
            yield return new WaitForSeconds(fadeTimeThatAffectsNothing / numFadeStepsThatAffectsNothing);
        }
        sr.sprite = null;
        Destroy(sr);

    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        if(!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
        }
        
    }

    /// <summary>
    /// If move camera input is detected, this method is called automatically
    /// </summary>
    private void MoveCamera_Started(InputAction.CallbackContext obj)
    {
        isCameraMoving = true;
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

        //Creates an outline of the range the camera can see at each position
        int arrayLength = paintingPositions.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            Gizmos.DrawWireCube(paintingPositions[i].transform.position, new Vector2(width, height));
        }
    }
}

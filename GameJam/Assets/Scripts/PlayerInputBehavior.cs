using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerInputBehavior : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput playerInput;
    InputAction pause;
    InputAction click;
    InputAction mPos;

    Vector2 mPosVector;
    [SerializeField] float spearItCooldDown;

    [Header("Sprites")]
    [SerializeField] GameObject punctureGameObject;
    [SerializeField] private Sprite tf2Coconut;

    [Header("Flashlight Variable")]
    [SerializeField] private GameObject lightObject;
    private Light2D flashlight;
    [SerializeField] private float maxMouseSpeed;

    [Header("Flashlight Flickering")]
    [SerializeField] private int maxFlickers;
    [SerializeField] private float lightFlickerReduction;
    [SerializeField] private int framesBetweenFlicker;
    [SerializeField] private int flickerPauseTime;
    [SerializeField] private float negativeRandomModifier;
    [SerializeField] private float positiveRandomModifier;

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

    //Time Variables
    private float time;
    private float lastClickTime;

    //Flicker Variables
    private float originalIntensity;
    private int flickerFrame;
    private int flickerPause;
    private int flickerCounter;
    public GameObject spawnCirc;

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
        mPos = playerInput.currentActionMap.FindAction("MousePos");

        //Binds actions to methods
        pause.performed += Pause_performed;
        click.performed += Click_performed;
        moveCamera.started += MoveCamera_Started;

        nothingToSeeHere.performed += contx => StartCoroutine(NothingToSeeHere_performed());

        //Initializes Camera variables
        isCameraMoving = false;
        currentPaintingIndex = startingPaintingIndex - 1;
        mainCamera.transform.position = paintingPositions[currentPaintingIndex].transform.position;

        lightObject.transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        flashlight = lightObject.GetComponent<Light2D>();

        //Initialzes time variables
        time = 0;
        lastClickTime = 0;

        //Initializes flicker variables
        originalIntensity = flashlight.intensity;
        flickerFrame = framesBetweenFlicker;
        flickerPause = 0;
        flickerCounter = 0;

    }

    public void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        HandleLightMovement();
        if(!FindObjectOfType<GameManager>().CamIsShaking)
        {
            HandleCameraMovement();
        }
        HandleLightFlicker();
        mPosVector = mPos.ReadValue<Vector2>();
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
        lightObject.transform.position = Vector2.MoveTowards(lightObject.transform.position, mousePosition, maxMouseSpeed);
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

    /// <summary>
    /// Handles the flickering of the light
    /// </summary>
    private void HandleLightFlicker()
    {
        //If Spear-It attack is on cooldown
        if (time <= lastClickTime + spearItCooldDown && lastClickTime != 0)
        {
            //If flicker count is at 3, pause for a moment then resume
            if (flickerCounter < maxFlickers)
            {
                //If light is not normal, make it normal if set amount of time has passed
                if (flashlight.intensity != originalIntensity && flickerFrame >= framesBetweenFlicker)
                {
                    flashlight.intensity = originalIntensity;
                    flickerFrame = 0;
                }
                //If light is normal, make it not normal if set amount of time has passed
                else if (flashlight.intensity == originalIntensity && flickerFrame >= framesBetweenFlicker)
                {
                    float lightIntensity = Random.Range(lightFlickerReduction - negativeRandomModifier, lightFlickerReduction + positiveRandomModifier);
                    flashlight.intensity = lightIntensity;
                    flickerFrame = 0;
                    flickerCounter++;
                }
                else
                    flickerFrame++;
            } else
            {
                flashlight.intensity = originalIntensity;
                if (flickerPause < flickerPauseTime)
                {
                    flickerPause++;
                } else
                {
                    flickerCounter = 0;
                    flickerPause = 0;
                }
            }
        } else
        {
            flashlight.intensity = originalIntensity;
        }
    }

    private void Click_performed(InputAction.CallbackContext obj)
    {
        //Cannot attack if cooldown is active or if player has not clicked yet (only relevant at beginning of the game)
        if (time >= lastClickTime + spearItCooldDown || lastClickTime == 0)
        {
            flickerCounter = 0;
            flickerPause = 0;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mPosVector), Vector3.zero);
            //Debug.DrawRay(mousePosition, Vector3.zero);
            //print(mPosVector);
            lastClickTime = time;
            if (hit.collider != null)
            {
                print(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Apparation")
                {
                    print("Entered");
                    Apparation aRef = FindObjectOfType<PaintingManager>().RetrieveApparationInstance(hit.transform.gameObject.name, hit.transform.gameObject);
                    if (aRef != null && aRef.IsApparating && !aRef.HasBeenCaught)
                    {
                        StopCoroutine(aRef.StartApparation());
                        aRef.Caught();
                        Instantiate(punctureGameObject, aRef.Sr.gameObject.transform.position, Quaternion.identity);
                        //Destroy(aRef.Sr.gameObject);   //Returns apparation to normal
                        FindObjectOfType<GameManager>().IncreaseScore();
                        //Stab animation
                    }
                    else if (aRef != null && aRef.HasApparated)
                    {
                        //Lose points
                    }
                }
            }

            Instantiate(punctureGameObject, mousePosition, Quaternion.identity);
            //TODO
        }
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
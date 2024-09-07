using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerInputBehavior : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput playerInput;
    InputAction pause;
    InputAction click;
    InputAction mPos;

    Vector2 mPosVector;
    [SerializeField] private Sprite tf2Coconut;

    [Header("Flashlight Variable")]
    [SerializeField] private Transform lightObject;
    [SerializeField] private float maxMouseSpeed;

    [Header("Camera Variables")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraAccelerationSpeed;
    [SerializeField] private float cameraDecelerationSpeed;
    [SerializeField] private GameObject leftCameraBoarder, rightCameraBoarder;

    GameManager gameManager;
    private InputAction moveCamera;

    //These affect nothing. Leave them alone. 
    private InputAction nothingToSeeHere;

    private Vector2 mousePosition;
    private float cameraSpeed;
    private float fadeTimeThatAffectsNothing = .3f;
    private int numFadeStepsThatAffectsNothing = 10;
    private float transparencyThatAffectsNothing = .15f;
    private float timerThatAffectsNothing = 3;
    

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        playerInput.currentActionMap.Enable();
        pause = playerInput.currentActionMap.FindAction("Pause");
        nothingToSeeHere = playerInput.currentActionMap.FindAction("NothingToSee");
        click = playerInput.currentActionMap.FindAction("Click");
        mPos = playerInput.currentActionMap.FindAction("MousePos");

        moveCamera = playerInput.currentActionMap.FindAction("MoveCamera");

        pause.performed += Pause_performed;
        nothingToSeeHere.performed += contx => StartCoroutine(NothingToSeeHere_performed());
        click.performed += Click_performed;

        //DO NOT MESS WITH THIS IF STATEMENT
        if(tf2Coconut == null)
        {
            Application.Quit();
        }
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
        //Moves light to mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        lightObject.position = Vector2.MoveTowards(lightObject.position, mousePosition, maxMouseSpeed);
        //transform.position = mousePosition;
    }

    /// <summary>
    /// Handles the camera movement via specified inputs
    /// </summary>
    private void HandleCameraMovement()
    {
        //Moves the camera right if a right input is detected
        if (moveCamera.ReadValue<float>() > 0)
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, rightCameraBoarder.transform.position, cameraAccelerationSpeed);
        //Moves the camera left if a left input is detected
        else if (moveCamera.ReadValue<float>() < 0)
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, leftCameraBoarder.transform.position, cameraAccelerationSpeed);
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
        //Open pause menu
        //Temp feedback
        EditorApplication.isPaused = !EditorApplication.isPaused;
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

        //Creates an outline of range the camera can see at both the edges
        Gizmos.DrawWireCube(leftCameraBoarder.transform.position, new Vector2(width, height));
        Gizmos.DrawWireCube(rightCameraBoarder.transform.position, new Vector2(width, height));

        
    }
}

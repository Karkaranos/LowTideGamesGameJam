
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    #endregion

    [SerializeField] public int expectedFrameRate = 60;
    [SerializeField] int score;
    [SerializeField] private int scoreNeededToWin;
    [SerializeField] int maxHealth;
    private int health;

    [Header("Visual References")]
    [SerializeField] private GameObject uncaughtApparationObj;
    [SerializeField] private Sprite landscapeApparation;
    [SerializeField] private Sprite portraitApparation;
    [SerializeField, Tooltip("UI Canvas Image for damage")] Image damageImage;
    [SerializeField] private Sprite[] damageVisuals;

    [Header("Time References")]
    [SerializeField, Tooltip("How long the uncaught apparation is visible for before it fades, in seconds")] private float timeBeforeApparationFades;
    [SerializeField, Tooltip("How long it takes the uncaught apparation to fade away")] private float apparationFadeTime;
    [SerializeField, Tooltip("How long before the camera starts shaking")] private float timeBeforeCamShake;
    [SerializeField, Tooltip("How long the camera shakes for")] private float camShakeTime;

    [Header("Camera Controls")]
     public bool CamIsShaking = false;
    [SerializeField, Range(0, 10), Tooltip("Camera shake intensity")] private int shakeIntensity;
    [SerializeField, Range(0, 10), Tooltip("Camera shake speed")] private int shakeSpeed;
    [SerializeField] private Camera mainCam;
    private int numTimerSteps = 100;

    [Header("Lighting")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D flashLight;
    [SerializeField] private float globalLightIncreaseRate;
    [SerializeField] private float delayforNextDecrease;

    [Header("Flashlight Flickering")]
    [SerializeField] private float flickerLength;
    [SerializeField] private int maxFlickers;
    [SerializeField] private float lightFlickerReduction;
    [SerializeField] private int framesBetweenFlicker;
    [SerializeField] private int flickerPauseTime;
    [SerializeField] private float negativeRandomModifier;
    [SerializeField] private float positiveRandomModifier;

    private bool transitionToDay;
    private int transitionFrames;

    //Flicker Variables
    private float originalIntensity;
    private int flickerFrame;
    private int flickerPause;
    private int flickerCounter;

    private float time;
    private float timeWon;
    public bool won {  get; private set; }
    private void Start()
    {
        transitionToDay = false;
        flickerFrame = 0;
        flickerPause = 0;
        flickerCounter = 0;
        time = 0;
        timeWon = 0;
        won = false;
        originalIntensity = flashLight.intensity;
        health = maxHealth;
        transitionFrames = 0;
        score = 9;
        IncreaseScore();
        
    }
    public void IncreaseScore()
    {
        score++;
        if (score >= scoreNeededToWin)
            WinGame();
    }

    public IEnumerator TakeDamage(Painting painting)
    {
        print("Roar!");
        SpriteRenderer sr = uncaughtApparationObj.GetComponent<SpriteRenderer>();
        if(painting.Type == Painting.PaintingType.LANDSCAPE)
        {
            sr.sprite = landscapeApparation;
        }
        else
        {
            sr.sprite = portraitApparation;
        }
        sr.gameObject.transform.position = painting.PaintingObj.transform.position;
        Color c = new Color(1, 1, 1, 1.0f);
        sr.color = c;
        health-=Mathf.Clamp((painting.NumApparationsComplete - painting.NumApparationsCaught - painting.DamagePointsDealt), 0, 10);
        painting.DamagePointsDealt += painting.NumApparationsComplete - painting.NumApparationsCaught;
        if (health <= 0)
        {
            EndGame();
        }
        print("New health: " + health);
        StartCoroutine(CameraShake());
        health--;
        yield return new WaitForSeconds(timeBeforeApparationFades);
        for(int i=0; i<numTimerSteps; i++)
        {
            c.a -= (1 / apparationFadeTime) * (apparationFadeTime / numTimerSteps);
            sr.color = c;
            yield return new WaitForSeconds(apparationFadeTime / numTimerSteps);
        }
        sr.color = new Color(1, 1, 1, 0);
        sr.gameObject.transform.position = new Vector3(0, 10, 0);
    }

    IEnumerator CameraShake()
    {
        yield return new WaitForSeconds(timeBeforeCamShake);
        CamIsShaking = true;
        //damageImage.sprite = damageVisuals[(maxHealth - health)];

        float timer = 0;
        while (timer < camShakeTime)
        {
            mainCam.transform.position = new Vector3(Mathf.PerlinNoise(0, Time.time * shakeSpeed) * 2 - 1,
                    Mathf.PerlinNoise(1, Time.time * shakeSpeed) * 2 - 1,
                    -10);
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CamIsShaking = false;

    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        if (time <= flickerLength + timeWon)
        {
            //If flicker count is at 3, pause for a moment then resume
            if (flickerCounter < maxFlickers)
            {
                //If light is not normal, make it normal if set amount of time has passed
                if (flashLight.intensity != originalIntensity && flickerFrame >= framesBetweenFlicker)
                {
                    flashLight.intensity = originalIntensity;
                    flickerFrame = 0;
                }
                //If light is normal, make it not normal if set amount of time has passed
                else if (flashLight.intensity == originalIntensity && flickerFrame >= framesBetweenFlicker)
                {
                    float lightIntensity = Random.Range(lightFlickerReduction - negativeRandomModifier, lightFlickerReduction + positiveRandomModifier);
                    flashLight.intensity = lightIntensity;
                    flickerFrame = 0;
                    flickerCounter++;
                }
                else
                    flickerFrame++;
            }
            else
            {
                flashLight.intensity = originalIntensity;
                if (flickerPause < flickerPauseTime)
                {
                    flickerPause++;
                }
                else
                {
                    flickerCounter = 0;
                    flickerPause = 0;
                }
            }
        }
        else
        {
            flashLight.enabled = false;
        }

        //Transitions darkness to brightness
        if (transitionToDay)
        {
            if (transitionFrames >= delayforNextDecrease && globalLight.intensity < 1)
            {
                globalLight.intensity += globalLightIncreaseRate;
                transitionFrames = 0;
            }
            else
                transitionFrames++;
        }
    }

    void WinGame()
    {
        won = true;
        //globalLight.color = new Color();
        timeWon = time;
        transitionToDay = true;
    }

    void EndGame()
    {
        Debug.Log("Animation here. You died tho");
    }
}
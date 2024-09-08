
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    [SerializeField] public int score;
    [SerializeField] private int scoreNeededToWin;
    [SerializeField] int maxHealth;
    private int health;

    [Header("Visual References")]
    [SerializeField] private GameObject uncaughtApparationObj;
    [SerializeField] private Sprite landscapeApparation;
    [SerializeField] private Sprite portraitApparation;
    //[SerializeField, Tooltip("UI Canvas Image for damage")] Image damageImage;
    [SerializeField] private GameObject[] damageVisuals;
    private int dmgTracker;

    [Header("Time References")]
    [SerializeField, Tooltip("How long the uncaught apparation is visible for before it fades, in seconds")] private float timeBeforeApparationFades;
    [SerializeField, Tooltip("How long it takes the uncaught apparation to fade away")] private float apparationFadeTime;
    [SerializeField, Tooltip("How long before the camera starts shaking")] private float timeBeforeCamShake;
    [SerializeField, Tooltip("How long the camera shakes for")] private float camShakeTime;

    [Header("Camera Controls")]
     public bool CamIsShaking = false;
    [SerializeField, Range(0, 5), Tooltip("Camera shake speed")] private float shakeIntensity;
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

    [Header("Sound")]
    [Tooltip("In Percentage, per frame. For example, 5 = 5% chance of any creepy extra happening in a given frame"), Range(0, 100)]
    [SerializeField] private float frequencyOfCreepyExtra;
    [SerializeField] private int timeToWaitToChanceSound;
    [SerializeField] private int collectionModifier;

    [Header("UI")]
    [SerializeField] private Image sanityMeter;
    [SerializeField] private Sprite[] sanitySprites;

    private int transitionFrames;

    //Flicker Variables
    private float originalIntensity;
    private int flickerFrame;
    private int flickerPause;
    private int flickerCounter;

    private float time;
    private float soundTimer;
    private float timeWon;
    public bool won {  get; private set; }

    AudioManager audioManager;
    private bool hasTriggered = false;

    public bool isScaring = false;
    private void Start()
    {
        audioManager = AudioManager.Instance;
        flickerFrame = 0;
        flickerPause = 0;
        flickerCounter = 0;
        time = 0;
        soundTimer = 0;
        timeWon = 0;
        won = false;
        originalIntensity = flashLight.intensity;
        health = maxHealth;
        transitionFrames = 0;
        
    }
    public void IncreaseScore()
    {
        score++;
        if (score >= scoreNeededToWin)
        {
            audioManager.Stop("Creepy Ambience");
            WinGame();
        }
    }

    public IEnumerator TakeDamage(Painting painting)
    {
        if(!won)
        {
            isScaring = true;
            //yield return new WaitForSeconds(1f);
            audioManager.Play("Take Damage");
            SpriteRenderer sr = uncaughtApparationObj.GetComponent<SpriteRenderer>();
            if (painting.Type == Painting.PaintingType.LANDSCAPE)
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
            int healthLost = Mathf.Clamp((painting.NumApparationsComplete - painting.NumApparationsCaught - painting.DamagePointsDealt), 0, 5);
            health -= healthLost;
            scoreNeededToWin -= healthLost;
            if(scoreNeededToWin < 7)
            {
                scoreNeededToWin = 7;
            }
            if (healthLost > 0)
            {
                //print("New health: " + health + " after taking " + (painting.NumApparationsComplete - painting.NumApparationsCaught - painting.DamagePointsDealt) + " points of damage");
                painting.DamagePointsDealt += painting.NumApparationsComplete - painting.NumApparationsCaught;
                if (health > 0)
                {
                    sanityMeter.sprite = sanitySprites[health - 1];
                }
                StartCoroutine(CameraShake());
                yield return new WaitForSeconds(timeBeforeApparationFades);
                for (int i = 0; i < numTimerSteps; i++)
                {
                    c.a -= (1 / apparationFadeTime) * (apparationFadeTime / numTimerSteps);
                    sr.color = c;
                    yield return new WaitForSeconds(apparationFadeTime / numTimerSteps);
                }
                sr.color = new Color(1, 1, 1, 0);
                sr.gameObject.transform.position = new Vector3(0, 10, 0);
            }
            isScaring = false;
        }
    }

    IEnumerator CameraShake()
    {
        yield return new WaitForSeconds(timeBeforeCamShake);
        CamIsShaking = true;
        //damageImage.sprite = damageVisuals[(maxHealth - health)];
        damageVisuals[dmgTracker++].SetActive(true);
        Vector3 camPos = mainCam.transform.position;
        float timer = 0;
        while (timer < camShakeTime)
        {
            mainCam.transform.position = new Vector3((Mathf.PerlinNoise(0, Time.time * shakeIntensity) * 2 - 1) + camPos.x,
                    Mathf.PerlinNoise(1, Time.time * shakeIntensity) * 2 - 1,
                    -10);
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (health <= 0)
        {
            audioManager.Stop("Creepy Ambience");
            audioManager.Play("Death");
            EndGame();
        }
        CamIsShaking = false;

    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        soundTimer += Time.fixedDeltaTime;

        //Sound
        //For every point you get, increase the frequency of potentially getting creepy extra
        if (soundTimer >= timeToWaitToChanceSound - ( score * collectionModifier ) )
        {
            soundTimer = 0;
            float randomChance = Random.Range(0, 100);

            //Chance check
            if (randomChance < frequencyOfCreepyExtra)
            {
                int randomClip = Random.Range(0, 4);

                switch (randomClip)
                {
                    case 0:
                        audioManager.Play("Creepy Extra 1");
                        break;
                    case 1:
                        audioManager.Play("Creepy Extra 2");
                        break;
                    case 2:
                        audioManager.Play("Creepy Extra 3");
                        break;
                    case 3:
                        audioManager.Play("Creepy Extra 4");
                        break;
                    default:
                        //print("ERROR: FAILED TO GET RANDOM SOUND");
                        break;
                }
            }
        }

        if (score >= scoreNeededToWin && !hasTriggered)
        {
            hasTriggered = true;
            audioManager.Stop("Creepy Ambience");
            WinGame();
        }

        //Victory End Flickering
        if (won)
        {
            print("jdskhgd");

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
                SceneManager.LoadScene("MainMenu");
            }


            //Transitions darkness to brightness

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
        audioManager.Play("Victory Jingle");
        won = true;
        FindObjectOfType<Constants>().IsGalleryClickable = true;
        //globalLight.color = new Color();
        timeWon = time;
    }

    void EndGame()
    {
        audioManager.Play("Loss Jingle");
        SceneManager.LoadScene("DeathScene");
    }
}
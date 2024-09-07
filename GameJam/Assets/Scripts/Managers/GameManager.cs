using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    [HideInInspector] public bool CamIsShaking = false;
    [SerializeField, Range(0, 10), Tooltip("Camera shake intensity")] private int shakeIntensity;
    [SerializeField, Range(0, 10), Tooltip("Camera shake speed")] private int shakeSpeed;
    private Camera mainCam;
    private int numTimerSteps;

    private void Start()
    {
        health = maxHealth;
        mainCam = Camera.main;
    }
    public void IncreaseScore()
    {
        score++;
    }

    public IEnumerator TakeDamage(Painting painting)
    {
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
        Color c = new Color(1, 1, 1, 1);
        sr.color = c;
        StartCoroutine(CameraShake());
        yield return new WaitForSeconds(timeBeforeApparationFades);
        for(int i=0; i<numTimerSteps; i++)
        {
            c.a -= (1 / apparationFadeTime) * (apparationFadeTime / numTimerSteps);
            sr.color = c;
        }
        sr.color = new Color(1, 1, 1, 0);
        sr.gameObject.transform.position = new Vector3(0, 10, 0);
    }

    IEnumerator CameraShake()
    {
        yield return new WaitForSeconds(timeBeforeCamShake);
        health--;
        if(health <= 0)
        {
            EndGame();
        }

        damageImage.sprite = damageVisuals[(maxHealth - health)];

        float timer = camShakeTime;
        while(timer < camShakeTime)
        {
            mainCam.transform.localPosition = new Vector3(Mathf.PerlinNoise(0,Time.time * shakeSpeed) * 2 -1,
                    Mathf.PerlinNoise(1, Time.time * shakeSpeed) * 2 - 1,
                    Mathf.PerlinNoise(2, Time.time * shakeSpeed) * 2 - 1) * .5f;
            timer += Time.deltaTime;
        }

    }

    void EndGame()
    {
        Debug.Log("Animation here. You died tho");
    }
}
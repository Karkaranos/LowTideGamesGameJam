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
     public bool CamIsShaking = false;
    [SerializeField, Range(0, 10), Tooltip("Camera shake intensity")] private int shakeIntensity;
    [SerializeField, Range(0, 10), Tooltip("Camera shake speed")] private int shakeSpeed;
    [SerializeField] private Camera mainCam;
    private int numTimerSteps = 100;

    private void Start()
    {
        health = maxHealth;
        StartCoroutine(TakeDamage(FindObjectOfType<PaintingManager>().Paintings[0]));
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
        Color c = new Color(1, 1, 1, 1.0f);
        sr.color = c;
        StartCoroutine(CameraShake());
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
        health--;
        if(health <= 0)
        {
            EndGame();
        }
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

    void EndGame()
    {
        Debug.Log("Animation here. You died tho");
    }
}
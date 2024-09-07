using UnityEngine;

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

    void Start()
    {
        
    }

    public void IncreaseScore()
    {
        score++;
    }
}
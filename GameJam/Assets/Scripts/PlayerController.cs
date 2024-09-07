using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    private static PlayerController instance;

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(PlayerController)) as PlayerController;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [SerializeField] private PlayerController playerController;
    void Start()
    {
        
    }
}
using UnityEngine;

public class Constants : MonoBehaviour
{
    public bool IsGalleryClickable = false;
    #region Singleton
    private static Constants instance;

    public static Constants Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(Constants)) as Constants;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion
}
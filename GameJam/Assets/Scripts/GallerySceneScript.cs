using UnityEngine;
using UnityEngine.SceneManagement;

public class GallerySceneScript : MonoBehaviour
{
    public void LeaveGallery()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

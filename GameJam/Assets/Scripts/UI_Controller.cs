using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{
    public GameObject CreditsCanvas;
    public GameObject MainMenuCanvas;

    public void CreditButtonPress() 
    {
        CreditsCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
    }

    public void BackToMainMenu()
    {
        CreditsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    public GameObject CreditsCanvas;
    public GameObject MainMenuCanvas;
    public GameObject TutorialCanvas;
    public TutorialPage[] TutorialPages;
    public Image TutorialIcon;
    public TMP_Text Title;
    public TMP_Text Body;

    public void CreditButtonPress() 
    {
        CreditsCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
        TutorialCanvas.SetActive(false);
    }

    public void BackToMainMenu()
    {
        CreditsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        TutorialCanvas.SetActive(false);
    }

    public void StartGameTutorial()
    { 
    TutorialCanvas.SetActive(true);
    MainMenuCanvas.SetActive(false);
    CreditsCanvas.SetActive(false) ;
    }
}

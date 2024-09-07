using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UI_Controller : MonoBehaviour
{
    public GameObject CreditsCanvas;
    public GameObject MainMenuCanvas;
    public GameObject TutorialCanvas;
    public TutorialPage[] TutorialPages;
    public Image TutorialIcon;
    public TMP_Text Title;
    public TMP_Text Body;

    public PlayerInput playerInput;
    private InputAction navigateForward;
    private InputAction navigateBackwards;

    private void Start()
    {
        playerInput.currentActionMap.Enable();
        navigateForward = playerInput.currentActionMap.FindAction("Progress");
        navigateBackwards = playerInput.currentActionMap.FindAction("MoveBack");
    }

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

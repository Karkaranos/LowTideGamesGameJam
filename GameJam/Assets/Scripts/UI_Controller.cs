using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class UI_Controller : MonoBehaviour
{
    AudioManager audioManager;

    public GameObject CreditsCanvas;
    public GameObject MainMenuCanvas;
    public GameObject TutorialCanvas;
    public GameObject AButton;
    public GameObject DButton;
    public Image[] Steppers;
    public Sprite EmptyStepper;
    public Sprite FilledStepper;
    public TutorialPage[] TutorialPages;
    public Image TutorialIcon;
    public TMP_Text Title;
    public TMP_Text Body;
    private int CurrentPage;

    public PlayerInput playerInput;
    private InputAction navigateForward;
    private InputAction navigateBackwards;
    public GameObject BeginButton;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        audioManager.Play("Menu Music");
        playerInput.currentActionMap.Enable();
        navigateForward = playerInput.currentActionMap.FindAction("Progress");
        navigateBackwards = playerInput.currentActionMap.FindAction("MoveBack");
        navigateBackwards.performed += NavigateBackwards_performed;
        navigateForward.performed += NavigateForward_performed;
    }

    private void NavigateForward_performed(InputAction.CallbackContext obj)
    {
        if(CurrentPage < TutorialPages.Length - 1)
        {
            audioManager.Play("UI Click");
            CurrentPage = CurrentPage + 1;
            DisplayPage();
            AButton.SetActive(true);

            if (CurrentPage == TutorialPages.Length - 1)
            {
                DButton.SetActive(false);
                BeginButton.SetActive(true);
                for (int i = 0; i < Steppers.Length; i++)
                {
                    Steppers[i].gameObject.SetActive(false);

                }
            }
        }
        
    }

    public void StartGame()
    {
        audioManager.Stop("Menu Music");
        SceneManager.LoadScene("LightScene");
    }
    private void DisplayPage()
    {
        Title.text = TutorialPages[CurrentPage].PageTitle;
        Body.text = TutorialPages[CurrentPage].BodyText;
        TutorialIcon.sprite = TutorialPages[CurrentPage].PageIcon;

        for (int i = 0; i < Steppers.Length; i++)
        {
            Steppers[i].sprite = EmptyStepper;
        }

        Steppers[CurrentPage].sprite = FilledStepper;
    }

    private void NavigateBackwards_performed(InputAction.CallbackContext obj)
    {
        if(CurrentPage > 0) 
        { 
            CurrentPage = CurrentPage - 1; 
            DisplayPage();
            BeginButton.SetActive(false);
            DButton.SetActive(true);

            if (CurrentPage == TutorialPages.Length - 2)
            {
                for (int i = 0; i < Steppers.Length; i++)
                {
                    Steppers[i].gameObject.SetActive(true);

                }
            }
            if (CurrentPage == 0)
            {
                AButton.SetActive(false);
            }
        }
    }

    public void CreditButtonPress() 
    {
        audioManager.Play("UI Click");
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
    CreditsCanvas.SetActive(false);

    CurrentPage = 0;
    DisplayPage();
    }

    public void GoToGallery()
    {
        audioManager.Play("UI Click");
        SceneManager.LoadScene("GalleryScene");
    }

    public void LeaveGallery()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        audioManager.Play("UI Click");
        Application.Quit();
    }
}
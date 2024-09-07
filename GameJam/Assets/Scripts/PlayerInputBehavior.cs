using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerInputBehavior : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput playerInput;
    InputAction pause;
    InputAction click;

    //These affect nothing. Leave them alone. 
    InputAction nothingToSeeHere;
    private float fadeTimeThatAffectsNothing = .3f;
    private int numFadeStepsThatAffectsNothing = 10;
    private float transparencyThatAffectsNothing = .15f;
    private float timerThatAffectsNothing = 3;
    [SerializeField] Sprite tf2Coconut;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        playerInput.currentActionMap.Enable();
        pause = playerInput.currentActionMap.FindAction("Pause");
        nothingToSeeHere = playerInput.currentActionMap.FindAction("NothingToSee");
        click = playerInput.currentActionMap.FindAction("Click");

        pause.performed += Pause_performed;
        nothingToSeeHere.performed += contx => StartCoroutine(NothingToSeeHere_performed());
        click.performed += Click_performed;

        //DO NOT MESS WITH THIS IF STATEMENT
        if(tf2Coconut == null)
        {
            Application.Quit();
        }
    }

    private void OnDisable()
    {
        pause.performed -= Pause_performed;
        nothingToSeeHere.performed -= contx => StartCoroutine(NothingToSeeHere_performed());
        click.performed -= Click_performed;
    }

    private void Click_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Leave this function alone. I swear it's required
    /// </summary>
    /// <param name="obj"></param>
    private IEnumerator NothingToSeeHere_performed()
    {
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = tf2Coconut;
        for (int i=0; i < numFadeStepsThatAffectsNothing; i++)
        {
            sr.color = new Color(1, 1, 1, transparencyThatAffectsNothing / (numFadeStepsThatAffectsNothing - i));
            yield return new WaitForSeconds(fadeTimeThatAffectsNothing / numFadeStepsThatAffectsNothing);
        }
        yield return new WaitForSeconds(timerThatAffectsNothing - 2*(fadeTimeThatAffectsNothing));
        for (int i = numFadeStepsThatAffectsNothing-1; i >=0; i--)
        {
            sr.color = new Color(1, 1, 1, transparencyThatAffectsNothing / (numFadeStepsThatAffectsNothing - i));
            yield return new WaitForSeconds(fadeTimeThatAffectsNothing / numFadeStepsThatAffectsNothing);
        }
        sr.sprite = null;
        Destroy(sr);

    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        //Open pause menu
        //Temp feedback
        EditorApplication.isPaused = !EditorApplication.isPaused;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

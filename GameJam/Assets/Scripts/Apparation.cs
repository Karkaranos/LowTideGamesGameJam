using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Apparation
{
    [SerializeField, Tooltip("The object being apparated")] GameObject apparationObject;
    [SerializeField, Tooltip("How long in seconds until the apparation starts")] float timeUntilStart;
    [SerializeField, Tooltip("How long the apparation takes to complete")] float apparatingCompletionTime;
    [SerializeField, Range(0, 100), Tooltip("How far along the apparation is as a percent"), ReadOnly]  float currentApparationProgress;
    [SerializeField, Tooltip("The sprite it changes to")] Sprite apparation;
    [SerializeField, ReadOnly] bool hasBeenCaught;
    [SerializeField, Tooltip("True if the new fades in, false if the old fades out")] bool newFadeIn = true;
    bool hasApparated = false;
    bool isApparating = false;
    private int steps = 100;
    public bool HasBeenCaught { get => hasBeenCaught; set => hasBeenCaught = value; }
    public bool IsApparating { get => isApparating; }
    public float TimeUntilStart { get => timeUntilStart;}
    public float ApparatingCompletionTime { get => apparatingCompletionTime;}
    public float CurrentApparationProgress {set => currentApparationProgress = value; }
    public GameObject ApparationObject { get => apparationObject;}
    public SpriteRenderer Sr { get => sr; }
    public bool HasApparated { get => hasApparated;}

    SpriteRenderer sr;

    public IEnumerator StartApparation()
    {
        if (newFadeIn)
        {
            yield return new WaitForSeconds(timeUntilStart);
            GameObject newSprite = new GameObject();
            newSprite.transform.parent = ApparationObject.transform;
            newSprite.transform.localPosition = Vector3.zero;
            sr = newSprite.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            Color c = new Color(1, 1, 1, 0);
            Sr.color = c;
            Sr.sprite = apparation;
            isApparating = true;
            for (int i = 0; i < steps; i++)
            {
                c.a += (1 / apparatingCompletionTime) * (apparatingCompletionTime / steps);
                if (sr != null)
                {
                    Sr.color = c;
                }
                else
                {
                    break;
                }
                currentApparationProgress = i + 1;
                yield return new WaitForSeconds(apparatingCompletionTime / steps);
            }
        }
        else
        {
            
            GameObject newSprite = new GameObject();
            newSprite.transform.parent = ApparationObject.transform;
            newSprite.transform.localPosition = Vector3.zero;
            sr = newSprite.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            Color c = new Color(1, 1, 1, 1);
            Sr.color = c;
            Sr.sprite = apparation;
            yield return new WaitForSeconds(timeUntilStart);
            isApparating = true;
            for (int i = 0; i < steps; i++)
            {
                c.a -= (1 / apparatingCompletionTime) * (apparatingCompletionTime / steps);
                if (sr != null)
                {
                    Sr.color = c;
                }
                else
                {
                    break;
                }
                currentApparationProgress = i + 1;
                yield return new WaitForSeconds(apparatingCompletionTime / steps);
            }
        }
        isApparating = false;
        hasApparated = true;
    }

    public void Caught()
    {
        if(isApparating)
        {
            Debug.Log("Stopped!");
        }
        isApparating = false;
        hasBeenCaught = true;
    }
}

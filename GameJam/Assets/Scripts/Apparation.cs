using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Apparation
{
    string name = "Apparation";
    [SerializeField, Tooltip("The object being apparated")] GameObject apparationObject;
    [SerializeField, Tooltip("How long in seconds until the apparation starts")] float timeUntilStart;
    [SerializeField, Tooltip("How long the apparation takes to complete")] float apparatingCompletionTime;
    [SerializeField, Range(0, 100), Tooltip("How far along the apparation is as a percent")]  float currentApparationProgress;
    [SerializeField, Tooltip("The sprite it changes to")] Sprite apparation;
    [SerializeField] bool hasBeenCaught;
    bool hasApparated = false;
    private int steps = 100;
    public bool HasBeenCaught { get => hasBeenCaught; set => hasBeenCaught = value; }
    public float TimeUntilStart { get => timeUntilStart;}
    public float ApparatingCompletionTime { get => apparatingCompletionTime;}
    public float CurrentApparationProgress {set => currentApparationProgress = value; }

    public IEnumerator StartApparation()
    {
        yield return new WaitForSeconds(timeUntilStart);
        GameObject newSprite = new GameObject();
        newSprite.transform.parent = apparationObject.transform;
        newSprite.transform.localPosition = Vector3.zero;
        SpriteRenderer sr = newSprite.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 100;
        Color c = new Color(1, 1, 1, 0);
        sr.color = c;
        sr.sprite = apparation;
        for(int i=0; i<steps; i++)
        {
            c.a += (1 / apparatingCompletionTime) * (apparatingCompletionTime/steps);
            sr.color = c;
            currentApparationProgress = i / 100;
            yield return new WaitForSeconds(apparatingCompletionTime / steps);
        }
        hasApparated = true;
    }
}

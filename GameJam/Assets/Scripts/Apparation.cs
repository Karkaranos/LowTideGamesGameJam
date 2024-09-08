using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Apparation
{
    [SerializeField, Tooltip("The object being apparated")] GameObject apparationObject;
    private Sprite startingSprite;
    [SerializeField, Tooltip("How long in seconds until the apparation starts")] float timeUntilStart;
    [SerializeField, Tooltip("How long the apparation takes to complete")] float apparatingCompletionTime;
    [SerializeField, Range(0, 100), Tooltip("How far along the apparation is as a percent"), /*ReadOnly*/]  float currentApparationProgress;
    [ Tooltip("The sprite it changes to")] public Sprite apparation;
    [SerializeField, /*ReadOnly*/] bool hasBeenCaught;
    [SerializeField, Tooltip("True if apparation is larger than original or same size; false if apparation is smaller than original")] bool newFadeIn = true;
    [SerializeField, Tooltip("True if old fades out as new fades in")] bool crossfade = false;
    bool hasApparated = false;
    bool isApparating = false;
    private int steps = 100;
    [SerializeField] private Material lit;
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
        ApparationMono am = new ApparationMono();
        if (newFadeIn)
        {
            yield return new WaitForSeconds(timeUntilStart);
            GameObject newSprite = new GameObject();
            newSprite.transform.parent = ApparationObject.transform;
            newSprite.transform.localPosition = Vector3.zero;
            newSprite.transform.localScale = new Vector3(1, 1, 1);
            sr = newSprite.AddComponent<SpriteRenderer>();
            sr.material = lit;
            sr.sortingOrder = 5;
            Color c = new Color(1, 1, 1, 0);
            Sr.color = c;
            Sr.sprite = apparation;
            isApparating = true;
            Color cc = new Color(1, 1, 1, 1);
            newSprite.tag = "Apparation";
            BoxCollider2D bc2d = newSprite.AddComponent<BoxCollider2D>();
            bc2d.offset = apparationObject.GetComponent<BoxCollider2D>().offset;
            bc2d.size = apparationObject.GetComponent<BoxCollider2D>().size;
            for (int i = 0; i < steps; i++)
            {
                if(isApparating)
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
                    if(crossfade)
                    {
                        cc.a -= (1 / apparatingCompletionTime) * (apparatingCompletionTime / steps);
                        apparationObject.GetComponent<SpriteRenderer>().color = cc;
                    }
                }
                else
                {
                    c.a = 0;
                    sr.color = c;
                    apparationObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }

            }
        }
        else
        {
            GameObject newSprite = new GameObject();
            newSprite.tag = "Apparation";
            BoxCollider2D bc2d = newSprite.AddComponent<BoxCollider2D>();
            bc2d.offset = apparationObject.GetComponent<BoxCollider2D>().offset;
            bc2d.size = apparationObject.GetComponent<BoxCollider2D>().size;
            newSprite.transform.parent = ApparationObject.transform;
            newSprite.transform.localPosition = Vector3.zero;
            newSprite.transform.localScale = new Vector3(1, 1, 1);
            sr = newSprite.AddComponent<SpriteRenderer>();
            sr.material = lit;
            sr.sortingOrder = 5;
            Color c = new Color(1, 1, 1, 1);
            Sr.color = c;
            Sr.sprite = apparation;

            Color cc = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(timeUntilStart);
            isApparating = true;
            for (int i = 0; i < steps; i++)
            {
                if(isApparating)
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
                    if (crossfade)
                    {
                        cc.a += (1 / apparatingCompletionTime) * (apparatingCompletionTime / steps);
                        apparationObject.GetComponent<SpriteRenderer>().color = cc;
                    }
                }
                else
                {
                    c.a = 1;
                    sr.color = c;

                    apparationObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                }
            }
        }
        if(!hasBeenCaught)
        {
            isApparating = false;
            hasApparated = true;
            Painting p = am.GetPainting(apparation);
            p.NumApparationsComplete++;
            if(p.NumApparationsComplete + p.NumApparationsCaught >=3)
            {
                Debug.Log("TEST");
                am.TriggerPaintingDrag(p);
            }
        }
        am.IncreaseApparationCount();
    }

    public void Caught()
    {
        ApparationMono am = new ApparationMono();
        Painting p = am.GetPainting(apparation);
        p.NumApparationsCaught++;
        isApparating = false;
        hasBeenCaught = true;
    }


}


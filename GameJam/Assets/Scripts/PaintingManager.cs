using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] Painting[] paintings;
    [SerializeField, ReadOnly] int currPaintingNum;

    public bool PaintingCameraOverride = false;

    public Painting[] Paintings { get => paintings; set => paintings = value; }


    // Start is called before the first frame update
    void Start()
    {
        foreach(Painting p in Paintings)
        {
            foreach(Apparation a in p.Apparations)
            {
                StartCoroutine(a.StartApparation());
            }
        }
    }

    public Apparation RetrieveApparationInstance(string name, GameObject g)
    {
        foreach(Painting p in Paintings)
        {
            foreach(Apparation a in p.Apparations)
            {
                if(a.ApparationObject.name == name || g.GetComponent<SpriteRenderer>().sprite == a.apparation)
                {
                    return a;
                }
            }
        }
        return null;
    }

    public Painting RetrievePaintingInstance(Sprite s)
    {
        foreach (Painting p in Paintings)
        {
            foreach (Apparation a in p.Apparations)
            {
                if (a.apparation == s)
                {
                    return p;
                }
            }
        }
        return null;
    }

    private int GetPaintingIndex(Painting p)
    {
        for(int i=0; i<paintings.Length; i++)
        {
            if(paintings[i] == p)
            {
                return i;
            }
        }
        return -1;
    }

    public void AttackPlayer(Painting p)
    {
        Debug.Log("DO SOMETHING " + p.PaintingName);
        int index = GetPaintingIndex(p);
        Debug.Log(p.PaintingName + " is at index " + index);
    }

}

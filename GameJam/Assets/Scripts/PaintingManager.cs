using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] Painting[] paintings;
    [SerializeField, ReadOnly] int currPaintingNum;

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

    public Apparation RetrieveApparationInstance(string name)
    {
        foreach(Painting p in Paintings)
        {
            foreach(Apparation a in p.Apparations)
            {
                if(a.ApparationObject.name == name)
                {
                    return a;
                }
            }
        }
        return null;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] Painting[] paintings;
    [SerializeField, ReadOnly] int currPaintingNum;

    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Painting p in paintings)
        {
            foreach(Apparation a in p.Apparations)
            {
                StartCoroutine(a.StartApparation());
            }
        }
    }

    public Apparation RetrieveApparationInstance(string name)
    {
        foreach(Painting p in paintings)
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

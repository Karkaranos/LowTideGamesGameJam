using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] Painting[] paintings;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

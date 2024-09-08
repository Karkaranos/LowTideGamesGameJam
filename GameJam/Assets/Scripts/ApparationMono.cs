using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApparationMono : MonoBehaviour
{
    public Painting GetPainting(Sprite s)
    {
        return FindObjectOfType<PaintingManager>().RetrievePaintingInstance(s);
    }

    public void TriggerPaintingDrag(Painting p)
    {
        FindObjectOfType<PaintingManager>().AttackPlayer(p);
    }
}

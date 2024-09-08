using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] Painting[] paintings;
    [SerializeField, /*ReadOnly*/] int currPaintingNum;

    public bool PaintingCameraOverride = false;
    int totalApparations = 0;

    public Painting[] Paintings { get => paintings; set => paintings = value; }
    public int TotalApparations { get => totalApparations; set => totalApparations = value; }


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

    public Painting RetrievePaintingInstance(GameObject g)
    {
        foreach (Painting p in Paintings)
        {
            if(p.PaintingObj.Equals(g))
            {

                //print("Aquired painting " + p.PaintingName);
                return p;
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
                //print("Aquired painting index for " + p.PaintingName);
                return i;
            }
        }
        return -1;
    }

    public void AttackPlayer(Painting p)
    {
        //print("Attack player");
        int index = GetPaintingIndex(p);
        FindObjectOfType<PlayerInputBehavior>().CameraMovementOverride(index);
    }
}
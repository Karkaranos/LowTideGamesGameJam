using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Painting
{
    [SerializeField] Apparation[] apparations;
    int completeApparationCount;

    public Apparation[] Apparations { get => apparations;}
}

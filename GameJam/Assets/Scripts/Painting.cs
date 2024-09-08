using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Painting
{
    public string PaintingName;
    [SerializeField] PaintingType type;
    [SerializeField] GameObject paintingObj;
    [SerializeField] Apparation[] apparations;
    private int numApparationsComplete;
    public enum PaintingType
    {
            LANDSCAPE, PORTRAIT
    };

    public Apparation[] Apparations { get => apparations;}
    public PaintingType Type { get => type;}
    public GameObject PaintingObj { get => paintingObj;}
    public int NumApparationsComplete { get => numApparationsComplete; set => numApparationsComplete = value; }
}

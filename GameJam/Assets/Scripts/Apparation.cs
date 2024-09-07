using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Apparation
{
    string name = "Apparation";
    [SerializeField, Tooltip("The object being apparated")] GameObject apparationObject;
    [SerializeField, Tooltip("How long in seconds until the apparation starts")] float timeUntilStart;
    [SerializeField, Tooltip("How long the apparation takes to complete")] float apparatingTime;
    [SerializeField, Range(0, 100), Tooltip("How far along the apparation is as a percent")]  float currentApparationProgress;
    [SerializeField, Tooltip("The sprite it changes to")] Sprite apparation;
    [SerializeField] bool hasBeenCaught;

}

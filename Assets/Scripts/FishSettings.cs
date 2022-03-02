using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject {

    [Header ("Fish farm parameters")]
    public float FarmRadius = 1;
    public float FarmHeight = 3;
    public float PreferredCageDistance = 0.5f;
    public float SpawnRadius = 3;
    public int SpawnCount = 1000;

    // Settings
    [Header ("Fish parameters")]
    public float BodyLength = 1;
    public float Speed = 3;


    [Header ("Behaviour")]
    public float PreferredDistance = .66f;
    public float DetectionDistance = 3f;
    public float DirectionchangeWeight = 0.45f;
    public float CageWeight = 10f;
    public float SocialWeight = 0.1f;

}
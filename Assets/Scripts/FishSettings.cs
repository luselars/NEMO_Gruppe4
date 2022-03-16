using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject {

    [Header ("Fish farm parameters")]
    public float FarmRadius = 20f;
    public float FarmHeight = 20f;
    public float PreferredCageDistance = 1f;
    public float SpawnRadius = 7f;
    public int SpawnCount = 3000;
    public int Time = 16;
    public float I_L = 3.4f;
    public float I_U = 76.4f;
    public float Tmax = 16f;
    public float Tmin = 2f;

    // Settings
    [Header ("Fish parameters")]
    public float BodyLength = 1f;
    public float Speed = 0.9f;
    public float MaxStomachVolume = 120f; //in grams

    [Header ("Behaviour")]
    public float PreferredDistance = .66f;
    public float DetectionDistance = 3f;
    public float DirectionchangeWeight = 0.45f;
    public float CageWeight = 0.8f;
    public float SocialWeight = 0.4f;
    public float LightWeight = 0.5f;
    public float TempWeight = 0.6f;
    public float FeedingWeight = 0.005f;
    public float Tu = 16f;
    public float Tl = 13f;
    public float TempUpperSteep = 80f;
    public float TempLowerSteep = -20f;

}
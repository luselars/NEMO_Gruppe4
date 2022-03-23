using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject
{

    [Header("Fish farm parameters")]
    public float FarmRadius = 1;
    public float FarmHeight = 3;
    public float PreferredCageDistance = 0.5f;
    public float SpawnRadius = 3;
    public int SpawnCount = 1000;
    public int Time = 14;
    public float I_L = 3.4f;
    public float I_U = 600.4f;
    public float Tmax;
    public float Tmin;

    // Settings
    [Header("Fish parameters")]
    public float BodyLength = 1;
    public float Speed = 3;
    public float MaxStomachVolume = 120; //in grams
    public float FishProgress;

    [Header("Behaviour")]
    public float PreferredDistance = .66f;
    public float DetectionDistance = 3f;
    public float DirectionchangeWeight = 0.45f;

    public float CageWeight = 10f;
    public float SocialWeight = 0.1f;
    public float LightWeight = 0.4f;
    public float RandWeight;
    public float TempWeight;
    public float FeedingWeight = 0.005f;
    public float Tu;
    public float Tl;
    public float TempUpperSteep;
    public float TempLowerSteep;
}
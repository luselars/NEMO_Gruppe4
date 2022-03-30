using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject
{

    [Header("Fish farm parameters")]
    public float FarmRadius = 20;
    public float FarmHeight = 20;
    public float PreferredCageDistance = 0.5f;
    public float SpawnRadius = 3;
    public int SpawnCount = 7000;
    public int Time = 14;
    public float IlluminationLowerbound = 3.4f;
    public float IlluminationUpperbound = 76.4f;
    public float maximumOceanTemperature = 16f;
    public float minimumOceanTemperature = 2f;

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
    public float PreferredUpperTemperature = 16f;
    public float PreferredLowerTemperature = 13f;
    public float TempUpperSteep;
    public float TempLowerSteep;
}
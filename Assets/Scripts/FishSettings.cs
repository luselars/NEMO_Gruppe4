using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject {
    // Settings
    [Header ("Fish parameters")]
    public float targetWeight = 1;

    [Header ("Behaviour")]
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;

}
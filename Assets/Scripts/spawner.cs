using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class spawner : MonoBehaviour
{   
    public Fish prefab;
    public FishSettings settings;

    void Awake () {
        System.Random rand = new System.Random();

        for (int i = 0; i < settings.SpawnCount; i++) {
            Vector2 CirclePos = (UnityEngine.Random.insideUnitCircle * settings.SpawnRadius);
            double upperPos = (settings.FarmHeight / 2) + 3;
            double lowerPos = (settings.FarmHeight / 2) - 3;

            float YPos = (float)((rand.NextDouble() * (upperPos - lowerPos)) + (lowerPos));

            Vector3 pos = new Vector3(CirclePos.x, YPos, CirclePos.y);
            Fish fish = Instantiate(prefab);
            fish.transform.position = pos;
            //fish.transform.forward = Random.insideUnitSphere;

            float a = (float)Math.Atan((fish.transform.position.y) / (fish.transform.position.x));
            fish.transform.forward = new Vector3(-(float)Math.Sin(a), 0, (float)Math.Cos(a));
        }
    }
}

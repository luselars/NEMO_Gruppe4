using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{   
    public Fish prefab;
    public FishSettings settings;

    void Awake () {

        for (int i = 0; i < settings.SpawnCount; i++) {
            Vector3 pos = Random.onUnitSphere * settings.SpawnRadius;
            Fish fish = Instantiate(prefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
        }
    }
}

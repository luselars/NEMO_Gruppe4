using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{   
    public Fish prefab;
    public float spawnRadius;
    public int spawnCount;

    void Awake () {

        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = Random.onUnitSphere * spawnRadius;
            Fish fish = Instantiate(prefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
        }
    }
}

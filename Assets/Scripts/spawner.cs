using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public enum GizmoType { Never, SelectedOnly, Always }
    
    public Fish prefab;
    public float spawnRadius = 5;
    public int spawnCount = 10;

    void Awake () {
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Fish fish = Instantiate (prefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public enum GizmoType { Never, SelectedOnly, Always }
    
    public Fish prefab;
    public float spawnRadius = 1;
    public int spawnCount = 10;

    void Awake () {
        GameObject fishes = GameObject.FindGameObjectsWithTag("Fish")[0];

        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = Random.insideUnitSphere * 0.01f;
            Fish fish = Instantiate(prefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;

            fishes.GetComponent<SocialBehaviour>().Children.Add(fish.gameObject);
        }
    }
}

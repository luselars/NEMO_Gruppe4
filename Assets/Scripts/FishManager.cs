using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public FishSettings settings;
    public ComputeShader compute;
    Fish[] fish;

    void Start () {
        fish = FindObjectsOfType<Fish> ();
        foreach (Fish f in fish) {
            f.Initialize (settings);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

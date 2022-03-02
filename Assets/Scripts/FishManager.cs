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

    void Update() {
         if (fish != null) {

            int numfish = fish.Length;
            var FishData = new FishData[numfish];

            for (int i = 0; i < fish.Length; i++) {
                FishData[i].position = fish[i].transform.position;
                FishData[i].direction = fish[i].Vref;
            }

            var fishBuffer = new ComputeBuffer (numfish, sizeof (float) * 3 * 3 + sizeof (int)); //FishData.Size);
            fishBuffer.SetData (FishData);

            compute.SetBuffer (0, "fish", fishBuffer);
            compute.SetInt ("numFish", fish.Length);
            compute.SetFloat ("PreferredDist", settings.PreferredDistance);
            compute.SetFloat ("DetectionDist", settings.DetectionDistance);

            int threadGroups = Mathf.CeilToInt (numfish / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            fishBuffer.GetData (FishData);

            for (int i = 0; i < fish.Length; i++) {
                fish[i].Vso = FishData[i].Vso;

                fish[i].UpdateFish ();
            }
            fishBuffer.Release ();
        }
    }
    public struct FishData {
        public Vector3 position;
        public Vector3 direction;
        public Vector3 Vso;
        public int numDetectedFish;
    }
}

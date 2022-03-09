using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeding : MonoBehaviour
{
    public bool isFeeding = false;
    public GameObject feeding;
    public FishSettings settings;

    void Start()
    {
        feeding.transform.position = new Vector3(2, settings.FarmHeight, 2);
        feeding.transform.localScale = new Vector3(settings.FarmRadius/5, settings.FarmHeight*0.001f, settings.FarmRadius/5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

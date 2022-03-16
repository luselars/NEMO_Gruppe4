using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public FishSettings settings;
    public GameObject farm;
    public GameObject bottom;
    void Start()
    {
        farm.transform.localScale = new Vector3 (settings.FarmRadius, settings.FarmHeight/2, settings.FarmRadius);
        farm.transform.position = new Vector3(0, settings.FarmHeight/2, 0);
        bottom.transform.localScale = new Vector3 (settings.FarmRadius, 0.0001f, settings.FarmRadius);
    }

    /* Update is called once per frame
    void Update()
    {
        
    }*/
}

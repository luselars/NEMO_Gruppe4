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
        farm.transform.localScale = new Vector3 (settings.FarmRadius*2, settings.FarmHeight/2, settings.FarmRadius*2);
        farm.transform.position = new Vector3(0, settings.FarmHeight/2, 0);
        bottom.transform.localScale = new Vector3 (settings.FarmRadius*2, 0.0001f, settings.FarmRadius*2);

        //print(GameObject.FindGameObjectsWithTag("CageWalls")[0].GetComponent<MeshRenderer>().bounds.size.y);
        //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().bounds.size.y);
    }

    /* Update is called once per frame
    void Update()
    {
        
    }*/
}

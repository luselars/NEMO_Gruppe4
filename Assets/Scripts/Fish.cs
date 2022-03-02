using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    FishSettings settings;

    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;

    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vref = Vector3.zero;
    public Vector3 Sref = Vector3.zero;
    public int OtherFish = 0;

    Vector3 Vprev = Vector3.zero;

    public float ck;
    public float sk;


    // Cached
    Material material;
    Transform cachedTransform;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        cachedTransform = transform;
    }

    public void Initialize (FishSettings settings) {
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        //velocity = transform.forward * startSpeed;
    }

    public float Length = 1;

    public float Speed;


    Feeding feeding;

    public void Start() {
        transform.position = (Random.onUnitSphere*15);
        feeding = FindObjectOfType<Feeding>();
    }

    public void Update ()
    {        
        var distanceref = 0.5f;
        Vcage = new Vector3(0,0,0);

        Vector3 origo = new Vector3(0.0f, transform.position.y, 0.0f);
        
        float distance = Vector3.Distance(origo, transform.position);

        if (distance >= 9.5f)
        {
            Vcage = - (transform.position - origo).normalized;
        }



        if (transform.position.y >= 15) {
            Vcage += new Vector3(0, 9-transform.position.y, 0);
        }
        if (transform.position.y <= 1){
            Vcage += new Vector3(0, 1.0f-transform.position.y, 0);
        }/*
        if (transform.position.x >= 5f-distanceref){
            Vcage += new Vector3(4.5f-transform.position.x, 0, 0);
        }
        if (transform.position.x <= -5f+distanceref){
            Vcage += new Vector3(-4.5f-transform.position.x, 0, 0);
        }
        if (transform.position.z >= 5f-distanceref){
            Vcage += new Vector3(0, 0, 4.5f-transform.position.z);
        }
        if (transform.position.z <= -5f+distanceref){
            Vcage += new Vector3(0, 0, -4.5f-transform.position.z);
        }*/

        if (OtherFish > 0)
        {
            Sref = Sref / OtherFish;
            OtherFish = 0;
        }

        Vector3 Plane = new Vector3(Vref.x, 0.0f, Vref.z);
        if (Vector3.Angle(Plane, Vref) > 60)
        {   
            Vref += new Vector3(0.0f, 0.6f, 0.0f);
        }

        Vref = Vref*0.65f + (1.0f-0.65f)*(ck*Vcage + sk*Sref);
        Vref = Vref.normalized;
        transform.position += Vref*Time.deltaTime*Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);

        float angle = Vector3.Angle(Vprev, Vref);

        Vector3 VprevHor = new Vector3(Vprev.x, 0, Vprev.z);




        if(angle < 2)
        {
            Vprev = Vref;
        }

        if (angle > 60)
        {

        }
        
    }    

}

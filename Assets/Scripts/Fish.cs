using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    FishSettings settings;
    Feeding feeding;

    [HideInInspector]
    public Vector3 position;

    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vso = Vector3.zero;
    public Vector3 Vref = Vector3.zero;

    Vector3 Vprev = Vector3.zero;

    // weighing constants for cage and social behaviour
    public float ck;
    public float sk;

    // Cage definitions
    public float upperBound;
    public float lowerBound;
    public float radius;

    // value between 0 and 24 that signifies what time of day it is.
    public float time;

    // Cached
    Material material;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
    }

    public void Initialize (FishSettings settings) {
        this.settings = settings;
        //velocity = transform.forward * startSpeed;
    }

    public float Length = 1;
    public float Speed = 3;

    public void Start() {
        //feeding = FindObjectOfType<Feeding>();
    }

    public void UpdateFish () {
        
        //Vcage
        var distanceref = 0.5f;
        Vcage = new Vector3(0,0,0);

        Vector3 origo = new Vector3(0.0f, transform.position.y, 0.0f);
        
        float distance = Vector3.Distance(origo, transform.position);


        // Sets Behaviour of fish when too close to the edge of the cage. 
        if (distance >= radius)
        {
            Vcage = -(transform.position - origo).normalized;
        }
        if (transform.position.y >= (upperBound))
        {
            Vcage += new Vector3(0, upperBound - transform.position.y, 0);
        }
        if (transform.position.y <= lowerBound)
        {
            Vcage += new Vector3(0, lowerBound - transform.position.y, 0);
        }
        /*if (distance >= 10.0f-distanceref)
        {
            Vcage = - (transform.position - origo).normalized;
        }

        if (transform.position.y >= 9) {
            Vcage += new Vector3(0, 9-transform.position.y, 0);
        }
        if (transform.position.y <= 1){
            Vcage += new Vector3(0, 1.0f-transform.position.y, 0);
        }*/

        //update position
        Vref = Vprev*0.65f + (1.0f-0.65f) * (ck * Vcage + sk * Vso); //Vso);
        Vref = Vref.normalized;
        transform.position += Vref*Time.deltaTime*Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);
        Vprev = Vref;
        
        Vector3 VprevHor = new Vector3(Vprev.x, 0, Vprev.z);
        Vector3 VrefHor = new Vector3(Vref.x, 0, Vref.z);

        float HAngle = Vector3.Angle(VprevHor, VrefHor);

        Vprev = Vref;

        float maxY = Speed / 2;
        if(Vref.y > maxY)
        {
            float diff = Vref.y - maxY;
            float xfrac = Vref.x / (Vref.x + Vref.z);
            float zfrac = 1 - xfrac;

            Vref.x += diff * xfrac;
            Vref.z += diff * zfrac;
            Vref.y = maxY;
        }
        if (Vref.y < -maxY)
        {
            float diff = Vref.y + maxY;
            float xfrac = Vref.x / (Vref.x + Vref.z);
            float zfrac = 1 - xfrac;

            Vref.x += diff * xfrac;
            Vref.z += diff * zfrac;
            Vref.y = -maxY;
        }


        if (HAngle > 2)
        {
            Vprev = Quaternion.AngleAxis(HAngle-60, Vector3.up) * Vprev;
        }
    }    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vref = Vector3.zero;
    Vector3 Vprev = Vector3.zero;

    public float Length = 1;

    public float Speed = 5;


    public void Start() {
        transform.position = (Random.onUnitSphere*15);
    }

    

    public void Update () {

        var distanceref = 0.5f;
        Vcage = new Vector3(0,0,0);
        if (transform.position.y >= 9) {
            Vcage += new Vector3(0, 9-transform.position.y, 0);
        }
        if (transform.position.y <= 1){
            Vcage += new Vector3(0, 1.0f-transform.position.y, 0);
        }
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
        }

        Vcage = Vcage;
        Vref = Vref*0.4f + (1.0f-0.4f)*Vcage;
        Vref = Vref.normalized;
        transform.position += Vref*Time.deltaTime*Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);
        Vprev = Vref;
    }    

}

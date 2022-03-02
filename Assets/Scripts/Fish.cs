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

    [HideInInspector]
    Vector3 Vprev = Vector3.zero;

    // Cached
    Material material;
    [HideInInspector]
    public float Bodylength;
    [HideInInspector]
    public float Speed;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
    }

    public void Initialize (FishSettings settings) {
        this.settings = settings;
        Speed = settings.Speed;
        Bodylength = settings.BodyLength;
    }

    

    public void Start() {
        //feeding = FindObjectOfType<Feeding>();
    }

    public void UpdateFish () {
        
        //Vcage
        Vcage = new Vector3(0,0,0);

        Vector3 origo = new Vector3(0.0f, transform.position.y, 0.0f);
        
        float distance = Vector3.Distance(origo, transform.position);

        if (distance >= settings.FarmRadius/2-settings.PreferredCageDistance)
        {
            Vcage = - (transform.position - origo).normalized;
        }

        if (transform.position.y >= settings.FarmHeight-settings.PreferredCageDistance) {
            Vcage += new Vector3(0, 9-transform.position.y, 0);
        }
        if (transform.position.y <= settings.PreferredCageDistance){
            Vcage += new Vector3(0, 1.0f-transform.position.y, 0);
        }

        //update position
        Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(Vcage*settings.CageWeight + Vso*settings.SocialWeight);
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
            Vprev = Quaternion.AngleAxis(HAngle-2, Vector3.up) * Vprev;
        }
    }    

}

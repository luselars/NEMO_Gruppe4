using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fish : MonoBehaviour
{
    FishSettings settings;
    Feeding feeding;

    [HideInInspector]
    public Vector3 position;

    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vso = Vector3.zero;
    public Vector3 Vref = Vector3.zero;
    public Vector3 Vli = Vector3.zero;
    public Vector3 Vtemp = Vector3.zero;

    public float PreferredLightUpper;
    public float PreferredLightLower;

    public float PreferredSteepnessUpper;
    public float PreferredSteepnessLower;

    public float Izero;
    public float I;

    public float TGy;
    public float T;

    [HideInInspector]
    Vector3 Vprev = Vector3.zero;
    System.Random rand = new System.Random();

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


        

        // set lower bound for light as a random between set boundaries
        double upper = 7;
        double lower = 3.5;
        PreferredLightUpper = (float)((rand.NextDouble() * (upper - lower)) + (lower));

        upper = 4.5;
        lower = 1.1;
        PreferredLightLower = (float)((rand.NextDouble() * (upper - lower)) + (lower));

        upper = 7;
        lower = 1.7;
        PreferredSteepnessLower = -(float)((rand.NextDouble() * (upper - lower)) + (lower));

        upper = 409;
        lower = 406;
        PreferredSteepnessUpper = (float)((rand.NextDouble() * (upper - lower)) + (lower));

        Izero = ((settings.I_U - settings.I_L) / 2) * (float)Math.Sin((Math.PI / 12) * (settings.Time - 6)) + ((settings.I_U - settings.I_L) / 2) + settings.I_L;
    }

    public void Start() {
        //feeding = FindObjectOfType<Feeding>();
    }

    public void UpdateFish () {
        
        //Vcage
        Vcage = Vector3.zero;

        Vector3 origo = new Vector3(0.0f, transform.position.y, 0.0f);
        
        float distance = Vector3.Distance(origo, transform.position);

        if (distance >= settings.FarmRadius/2-settings.PreferredCageDistance)
        {
            Vcage = - (transform.position - origo).normalized;
        }

        if (transform.position.y >= settings.FarmHeight-settings.PreferredCageDistance) {
            Vcage += new Vector3(0, settings.FarmHeight-settings.PreferredCageDistance-transform.position.y, 0);
        }
        if (transform.position.y <= settings.PreferredCageDistance){
            Vcage += new Vector3(0, settings.PreferredCageDistance-transform.position.y, 0);
        }

        // Light stuff

        I = Izero * (float)(Math.Exp(0.07f * (transform.position.y - settings.FarmHeight)));

        Vector3 VliL = Vector3.zero;
        Vector3 VliU = Vector3.zero;

        if (PreferredLightUpper < I)
        {
            VliU = new Vector3(0, -1, 0) * ((I-PreferredLightUpper)/(PreferredSteepnessUpper-PreferredLightUpper));
        }
        else
        {
            VliU = Vector3.zero;
        }
        if(PreferredLightLower > I)
        {
            VliL = new Vector3(0, 1, 0) * ((PreferredLightLower - I) / (PreferredLightLower - PreferredSteepnessLower));
        }
        else
        {
            VliL = Vector3.zero;
        }

        Vli = (VliL + VliU);


        // Temperature stuff

        TGy = -((settings.Tmax - settings.Tmin) / 25) * (float)Math.Exp((transform.position.y - settings.FarmHeight) / 25);
        Vtemp = Vector3.zero;
        T = settings.Tmin + ((settings.Tmax - settings.Tmin) * (float)Math.Exp((transform.position.y - settings.FarmHeight)/25));

        if(T > settings.Tu)
        {
            Vtemp += new Vector3(0, TGy, 0) * ((T - settings.Tu) / (settings.TempUpperSteep - settings.Tu));
        }
        if(T < settings.Tl)
        {
            Vtemp += new Vector3(0, -TGy, 0) * ((settings.Tl - T) / (settings.Tl - settings.TempLowerSteep));
        }

        
        Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(Vcage*settings.CageWeight + Vso*settings.SocialWeight + Vli*settings.LightWeight + Vtemp*settings.TempWeight);
        Vref = Vref.normalized;

        // Angle updates
        Vector3 VprevHor = new Vector3(Vprev.x, 0, Vprev.z);
        Vector3 VrefHor = new Vector3(Vref.x, 0, Vref.z);

        float HAngle = Vector3.Angle(VprevHor, VrefHor);

        
        float currentSpeed = (float)((rand.NextDouble() * (Speed - (Speed - 0.1))) + (Speed - 0.1));
        Vref = Vref * currentSpeed;


        float maxY = currentSpeed / 2;
        if (Vref.y > maxY)
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
            Vprev = Quaternion.AngleAxis(HAngle - 2, Vector3.up) * Vprev;
        }

        //update position
        transform.position += Vref * Time.deltaTime;// *Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);
        Vprev = Vref;
        
        

        Vprev = Vref;

       
    }    

}

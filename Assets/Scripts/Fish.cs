using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathNet.Numerics;

public class Fish : MonoBehaviour
{
    FishSettings settings;
    Feeding feeding;
    [HideInInspector]
    public double mean;
    [HideInInspector]
    public double stdDev = 0.25;

    [HideInInspector]
    public Vector3 currentPosition;
    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vso = Vector3.zero;
    public Vector3 Vref = Vector3.zero;
    public Vector3 Vli = Vector3.zero;
    [HideInInspector]
    public Vector3 VliL = Vector3.zero;
    [HideInInspector]
    public Vector3 VliU = Vector3.zero;
    public Vector3 Vtemp = Vector3.zero;
    public Vector3 Vrand = Vector3.zero;

    public float Speed;
    [HideInInspector]
    public float preferredDist;
    [HideInInspector]
    public float detectionDist;
    [HideInInspector]
    public float PreferredLightUpper;
    [HideInInspector]
    public float PreferredLightLower;

    [HideInInspector]
    public float PreferredSteepnessUpper;
    [HideInInspector]
    public float PreferredSteepnessLower;

    [HideInInspector]
    public float Izero;
    [HideInInspector]
    public float I;
    
    [HideInInspector]
    public float TGy;
    [HideInInspector]
    public float T;

    [HideInInspector]
    Vector3 Vprev = Vector3.zero;
    System.Random rand = new System.Random();

    // Cached
    Material material;
    public float Bodylength;
    [HideInInspector]
    Vector3 VprevHor = Vector3.zero;
    [HideInInspector]
    Vector3 VrefHor = Vector3.zero;

    private float stomachVolume;
    private float probFoodDetection;
    private Vector3 feedingPos;
    private Vector3 xzPosFeedingObj;

    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        feeding = FindObjectOfType<Feeding>();
        Vector3 feedingPosition = feeding.transform.position;

    }

    public void Initialize(FishSettings settings)
    {
        this.settings = settings;
        Bodylength = settings.BodyLength;
        stomachVolume = settings.MaxStomachVolume * 0.5f;


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

        float TimeSine = (float)Math.Sin((Math.PI / 12) * (settings.Time - 6));

        Izero = ((settings.IlluminationUpperbound - settings.IlluminationLowerbound) / 2) * TimeSine +
                ((settings.IlluminationUpperbound - settings.IlluminationLowerbound) / 2) + settings.IlluminationLowerbound;

        // Define bodylength
        float length_mean = 0.65f * settings.FishProgress + 0.15f;

        double upperLength = length_mean + 0.07;
        double lowerLength = length_mean - 0.07;

        Bodylength = (float)((rand.NextDouble() * (upperLength - lowerLength)) + (lowerLength));

        detectionDist = 3f * Bodylength;
        preferredDist = 0.66f * Bodylength;

        double upperSpeed = 0.65;
        double lowerSpeed = 0.45;

        transform.localScale = transform.localScale * Bodylength;

        Speed = Bodylength * (float)((rand.NextDouble() * (upperSpeed - lowerSpeed)) + (lowerSpeed));

        if (TimeSine < 0)
        {
            Speed = Speed / 2;
        }
    }

    private Vector3 calculateVcage(Vector3 currentPosition) {
        Vcage = Vector3.zero;
        Vector3 origo = new Vector3(0.0f, currentPosition.y, 0.0f);
        float distance = Vector3.Distance(origo, currentPosition);
        if (distance >= settings.FarmRadius - settings.PreferredCageDistance) {
            Vcage -= (currentPosition - origo).normalized * (distance - (settings.FarmRadius - settings.PreferredCageDistance));
        }
        if (currentPosition.y >= settings.FarmHeight - settings.PreferredCageDistance) {
            Vcage += new Vector3(0, settings.FarmHeight - settings.PreferredCageDistance - currentPosition.y, 0);
        }
        if (currentPosition.y <= settings.PreferredCageDistance) {
            Vcage += new Vector3(0, settings.PreferredCageDistance - currentPosition.y, 0);
        }
        return Vcage;
    }

    // Light stuff
    private Vector3 calculateVli(Vector3 currentPosition) {
        I = Izero * (float)(Math.Exp(0.07f * (currentPosition.y - settings.FarmHeight)));

        if (PreferredLightUpper < I) {
            VliU = new Vector3(0, -1, 0) * ((I - PreferredLightUpper) / (PreferredSteepnessUpper - PreferredLightUpper));
        }
        else {
            VliU = Vector3.zero;
        }
        if (PreferredLightLower > I) {
            VliL = new Vector3(0, 1, 0) * ((PreferredLightLower - I) / (PreferredLightLower - PreferredSteepnessLower));
        }
        else{
            VliL = Vector3.zero;
        }

        return Vli = (VliL + VliU);
    }

    private Vector3 calculateVTemp(Vector3 currentPosition) {
        TGy = -((settings.maximumOceanTemperature - settings.minimumOceanTemperature) / 25) * (float)Math.Exp((currentPosition.y - settings.FarmHeight) / 25);
        T = settings.minimumOceanTemperature + ((settings.maximumOceanTemperature - settings.minimumOceanTemperature) * (float)Math.Exp((currentPosition.y - settings.FarmHeight) / 25));

        if (T > settings.PreferredUpperTemperature)
        {
            return new Vector3(0, TGy, 0) * ((T - settings.PreferredUpperTemperature) / (settings.TempUpperSteep - settings.PreferredUpperTemperature));
        } else if (T < settings.PreferredLowerTemperature)
        {
            return new Vector3(0, -TGy, 0) * ((settings.PreferredLowerTemperature - T) / (settings.PreferredLowerTemperature - settings.TempLowerSteep));
        } else {
            return Vector3.zero;
        }
    }

    private Vector3 calculateVrand(Vector3 Vprev) {
        double meanX = Vprev.x;
        double meanY = Vprev.y;
        double meanZ = Vprev.z;

        MathNet.Numerics.Distributions.Normal normalDistX = new MathNet.Numerics.Distributions.Normal(meanX, stdDev);
        MathNet.Numerics.Distributions.Normal normalDistY = new MathNet.Numerics.Distributions.Normal(meanY, stdDev);
        MathNet.Numerics.Distributions.Normal normalDistZ = new MathNet.Numerics.Distributions.Normal(meanZ, stdDev);

        return new Vector3((float)normalDistX.Sample(), (float)normalDistY.Sample(), (float)normalDistZ.Sample());
    }

    private void checkAngle(){
        VprevHor = VrefHor;
        VrefHor = new Vector3(Vref.x, 0, Vref.z);

        float horizontalAngle = Vector3.SignedAngle(VprevHor, VrefHor, Vector3.up);

        //Y angle
        float maxY = Speed / 2;
        if (Vref.y > maxY) {
            float diff = Vref.y - maxY;
            float xfrac = Vref.x / (Vref.x + Vref.z);
            float zfrac = 1 - xfrac;

            Vref.x += diff * xfrac;
            Vref.z += diff * zfrac;
            Vref.y = maxY;
        }
        if (Vref.y < -maxY) {
            float diff = Vref.y + maxY;
            float xfrac = Vref.x / (Vref.x + Vref.z);
            float zfrac = 1 - xfrac;

            Vref.x += diff * xfrac;
            Vref.z += diff * zfrac;
            Vref.y = -maxY;
        }

        float maxAngle = 60 * Time.deltaTime;

        if (horizontalAngle > maxAngle)
        {

            Vref = Quaternion.AngleAxis(maxAngle - horizontalAngle, Vector3.up) * Vref;
        }else if(horizontalAngle < -maxAngle)
        {
            print(horizontalAngle+ " " + (-maxAngle-horizontalAngle));
            Vref = Quaternion.AngleAxis(-maxAngle - horizontalAngle, Vector3.up) * Vref;
        }

    }

    public void UpdateFish() {
        currentPosition = transform.position;

        Vref = Vprev * settings.DirectionchangeWeight + (1.0f - settings.DirectionchangeWeight) * 
                    (calculateVcage(currentPosition) * settings.CageWeight +
                    Vso * settings.SocialWeight +
                    calculateVli(currentPosition) * settings.LightWeight +
                    calculateVTemp(currentPosition) * settings.TempWeight +
                    calculateVrand(Vprev) * settings.RandWeight);
        Vref = Vref.normalized;

        checkAngle();

        transform.position += Vref * Time.deltaTime*Speed*settings.Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);
        Vprev = Vref;
    }

    //prob increases when distance to feeding area decreases
    private float ProbFoodDetection()
    {
        return 1 / (1 + Vector3.Distance(feeding.transform.position, transform.position));
    }

    private float ProbFeelingHungry()
    {
        float probFeelHunger;
        float rand = UnityEngine.Random.Range(0.0096f, 120f);
        float Xnorm = (rand - 0.0096f) / (120f - 0.0096f); // normalised stomach volume

        if (Xnorm >= 0.3)
        {
            probFeelHunger = 0.5f - (0.57f * (Xnorm - 0.3f / Xnorm - 0.2f));
            return probFeelHunger;
        }
        else
        {
            probFeelHunger = 0.5f + (0.67f * (0.3f - Xnorm / 0.4f - Xnorm));
            return probFeelHunger;
        }
    }

    private float ProbPelletCapture()
    {
        Vector3 xzFeedingPos = new Vector3(feeding.transform.position.x, transform.position.y, feeding.transform.position.z);
        float radius = feeding.transform.localScale.x / 10f;
        bool isFishInsideFeedingRadius = Vector3.Distance(xzFeedingPos, transform.position) < radius;

        if (isFishInsideFeedingRadius)
        {
            float probPelletCapture = 0.05f / Mathf.Pow(1 + Vector3.Distance(feedingPos, transform.position), 3f);
            return probPelletCapture;
        }
        else
        {
            return 0f;
        }
    }
}

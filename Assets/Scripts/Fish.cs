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

    public float fishProbHunger; 
    public float fishProbFoodDetect; 
    public float fishProbPelletCapture; 
    public bool isFishInsideFeedingRadius;
    public float randTimer;

    [HideInInspector]
    public Vector3 currentPosition;
    public Vector3 feedingPosition;

    public float speedCorrection = 1.0f;

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
    public Vector3 VFeeding = Vector3.zero;

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

    public FeedingState currentFeedingState;
    public enum FeedingState {Normal, Satiated, Approach, Manipulate}

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

        fishProbHunger = ProbFeelingHungry2();
        randTimer = UnityEngine.Random.Range(5f, 30f);


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

    public void Start() {
        stomachVolume = settings.MaxStomachVolume*0.5f;
        currentFeedingState = FeedingState.Normal;
        feedingPosition = feeding.transform.position;
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
            return Vtemp = new Vector3(0, TGy, 0) * ((T - settings.PreferredUpperTemperature) / (settings.TempUpperSteep - settings.PreferredUpperTemperature));
        } else if (T < settings.PreferredLowerTemperature)
        {
            return Vtemp = new Vector3(0, -TGy, 0) * ((settings.PreferredLowerTemperature - T) / (settings.PreferredLowerTemperature - settings.TempLowerSteep));
        } else {
            return Vector3.zero;
        }
    }

    private Vector3 calculateVrand(Vector3 Vprev) {
        double meanX = Vprev.x;
        double meanY = Vprev.y;
        double meanZ = Vprev.z;

        try
        {
            
        }catch(Exception e)
        {
            print(e);
            return Vrand;
        }
            MathNet.Numerics.Distributions.Normal normalDistX = new MathNet.Numerics.Distributions.Normal(meanX, stdDev);
            MathNet.Numerics.Distributions.Normal normalDistY = new MathNet.Numerics.Distributions.Normal(meanY, stdDev);
            MathNet.Numerics.Distributions.Normal normalDistZ = new MathNet.Numerics.Distributions.Normal(meanZ, stdDev);

            return Vrand = new Vector3((float)normalDistX.Sample(), (float)normalDistY.Sample(), (float)normalDistZ.Sample());
        

        
    }

    private void checkAngle(){
        VprevHor = VrefHor;
        //VrefHor = new Vector3(Vref.x, 0, Vref.z);
        VrefHor.x = Vref.x;
        VrefHor.z = Vref.z;

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
            //print(horizontalAngle+ " " + Quaternion.AngleAxis(-maxAngle - horizontalAngle, Vector3.up));
            Vref = Quaternion.AngleAxis(-maxAngle - horizontalAngle, Vector3.up) * Vref;
        }
        VrefHor.x = Vref.x;
        VrefHor.z = Vref.z;

        return;
    }

    public void UpdateFish() {
        currentPosition = transform.position;
        FeedBehaviour(currentPosition, feedingPosition);

        if(false &&currentFeedingState==FeedingState.Approach && false)
        {
            VFeeding = (feedingPosition - currentPosition)*Time.deltaTime;
            Vref = Vprev * settings.DirectionchangeWeight + (1.0f - settings.DirectionchangeWeight) * 
                   (
                    calculateVcage(currentPosition) * settings.CageWeight +
                    Vso * settings.SocialWeight +
                    VFeeding * settings.FeedingWeight * fishProbHunger +
                    calculateVrand(Vprev) * settings.RandWeight
                    );

            ProbPelletCapture();
        } else if(false && currentFeedingState ==FeedingState.Manipulate && false)
        {
            VFeeding = (feedingPosition - currentPosition)*Time.deltaTime;
            Vref = Vprev * settings.DirectionchangeWeight + (1.0f - settings.DirectionchangeWeight) * 
                   (
                    calculateVcage(currentPosition) * settings.CageWeight +
                    Vso * settings.SocialWeight +
                    VFeeding * settings.FeedingWeight * fishProbHunger +
                    calculateVrand(Vprev) * settings.RandWeight
                    );
           
            if(randTimer>0.0f){
                currentFeedingState = FeedingState.Manipulate;
                randTimer -= Time.deltaTime;
            } else{
                fishProbHunger = ProbFeelingHungry2();
            }
        } else if (false && currentFeedingState ==FeedingState.Normal || currentFeedingState==FeedingState.Satiated )
        {
            VFeeding = (feedingPosition - currentPosition)*Time.deltaTime;
            Vref = Vprev * settings.DirectionchangeWeight + (1.0f - settings.DirectionchangeWeight) * 
                   (
                    calculateVcage(currentPosition) * settings.CageWeight +
                    Vso * settings.SocialWeight +
                    calculateVli(currentPosition) * settings.LightWeight * ( 1 - fishProbHunger )+
                    calculateVTemp(currentPosition) * settings.TempWeight * ( 1 - fishProbHunger ) +
                    new Vector3(0, VFeeding.y, 0) * settings.FeedingWeight * fishProbHunger +
                    calculateVrand(Vprev) * settings.RandWeight
                    );
        } else
        {
            VFeeding = (feedingPosition - currentPosition) * Time.deltaTime;

            Vector3 Vnew = (
                    calculateVcage(currentPosition) * settings.CageWeight +
                    Vso * settings.SocialWeight +
                    calculateVli(currentPosition) * settings.LightWeight * (1 - fishProbHunger) +
                    calculateVTemp(currentPosition) * settings.TempWeight * (1 - fishProbHunger) +
                    //new Vector3(0, VFeeding.y, 0) * settings.FeedingWeight * fishProbHunger +
                    calculateVrand(Vprev) * settings.RandWeight
                    );
            float VrefMagnitude = Vnew.magnitude;

            speedCorrection = 0.4f;
            print(VrefMagnitude);
            if (VrefMagnitude > 0.4f)
            {
                speedCorrection = 0.4f / VrefMagnitude;
            }
            if (VrefMagnitude < 0.3f)
            {
                speedCorrection = 0.3f / VrefMagnitude;
            }

            Vnew = Vnew * speedCorrection;

            Vnew = Vnew.normalized;


            Vref = Vprev * settings.DirectionchangeWeight + (1.0f - settings.DirectionchangeWeight) *
                   Vnew;
        }

        float VrefMagnitude2 = Vref.magnitude;

        

        Vref = Vref.normalized;

        checkAngle();

        // CAP SPEED

        

        Vref = Vref * VrefMagnitude2;


        transform.position += Vref * Time.deltaTime*settings.Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        Vprev = Vref;
    }

    public void FeedBehaviour(Vector3 currPos, Vector3 Fpos){
        if(!feeding.isFeeding){
            currentFeedingState = FeedingState.Normal; 
            if(fishProbHunger>0.05f && randTimer<0.0f) fishProbHunger = ProbFeelingHungry2();
            if(randTimer<0.0f) randTimer = UnityEngine.Random.Range(5f, 30f);
        }
        if(feeding.isFeeding)
        {
            if((fishProbHunger*ProbFoodDetection(feeding.anticipateFeeding))<0.5f){
                currentFeedingState = FeedingState.Satiated;
            }else if (currentFeedingState!=FeedingState.Satiated && (fishProbHunger*ProbFoodDetection(feeding.anticipateFeeding))>=0.5f){
                currentFeedingState = FeedingState.Approach;
            } else if (currentFeedingState!=FeedingState.Satiated && fishProbHunger>0.5f && randTimer>0.0f)
            {   currentFeedingState= FeedingState.Manipulate;
            }
        } 

        switch(currentFeedingState)
        {
            case FeedingState.Normal:
                break;
            case FeedingState.Satiated:
                break;
            case FeedingState.Approach: //move towards feeding area
                if(fishProbPelletCapture > 0.5f) currentFeedingState = FeedingState.Manipulate;
                break;
            case FeedingState.Manipulate:
                if(fishProbHunger<0.5f) currentFeedingState = FeedingState.Satiated;
                else if(fishProbHunger>0.5f && !(randTimer > 0.0f)) currentFeedingState = FeedingState.Approach;
                break;
            }
    }

    //prob increases when distance to feeding area decreases
    //prob is 1 if anticipateFeed is true
    float ProbFoodDetection(bool anticipateFeed)
    {
        if(anticipateFeed){
            return fishProbFoodDetect = 1;
        }else
        {
            float probFoodDetect =  1/(1+Vector3.Distance(feeding.transform.position, transform.position));
            return fishProbFoodDetect = probFoodDetect;
        }
    }

    //settes når programmet starter og igjen når timeren i Manipulate state blir 0
    public float ProbFeelingHungry2()
    {
        float meanHungerVar = 0.5f; //todo: user input - verdi mellom 0 og 1
        float std = 0.1f; //todo: user input
        float probHunger = UnityEngine.Random.Range(meanHungerVar - 3*std, meanHungerVar + 3*std ) ;
        if(probHunger>1) probHunger = 1;
        if(probHunger<0) probHunger = 0;      
        
        return probHunger;
    }

    private float ProbPelletCapture()
    {
        Vector3 xzFeedingPos = new Vector3(feedingPosition.x+1, currentPosition.y, feedingPosition.z+1);
        float radius = feeding.transform.localScale.x; // /10f
        isFishInsideFeedingRadius = Vector3.Distance(xzFeedingPos, transform.position) < radius;

        float randValue = UnityEngine.Random.Range(1, 101) ;
        if (isFishInsideFeedingRadius)
        {
            return fishProbPelletCapture = randValue/100;
        }else {
            return fishProbPelletCapture = 0f;
        }
    }
}

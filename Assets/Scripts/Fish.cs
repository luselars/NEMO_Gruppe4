using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fish : MonoBehaviour
{
    FishSettings settings;
    Feeding feeding;

    [HideInInspector]
    public Vector3 currentPosition;

    public Vector3 Vcage = Vector3.zero;
    public Vector3 Vso = Vector3.zero;
    public Vector3 Vref = Vector3.zero;
    public Vector3 Vli = Vector3.zero;
    public Vector3 Vtemp = Vector3.zero;
    public Vector3 VFeeding = Vector3.zero;
    

    public float PreferredLightUpper;
    public float PreferredLightLower;

    public float PreferredSteepnessUpper;
    public float PreferredSteepnessLower;

    public float Izero;
    public float I;

    public float TGy;
    public float T;

    private FeedingState currentFeedingState;
    private enum FeedingState {Normal, Satiated, Approach, Manipulate}

    [HideInInspector]
    Vector3 Vprev = Vector3.zero;
    System.Random rand = new System.Random();

    // Cached
    Material material;
    [HideInInspector]
    public float Bodylength;
    [HideInInspector]
    public float Speed;

    private float stomachVolume;
    private float probFoodDetection;
    private Vector3 feedingPos;
    private Vector3 xzPosFeedingObj;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        feeding = FindObjectOfType<Feeding>();
        Vector3 feedingPosition = feeding.transform.position;
        
    }

    public void Initialize (FishSettings settings) {
        this.settings = settings;
        Speed = settings.Speed;
        Bodylength = settings.BodyLength;
        stomachVolume = settings.MaxStomachVolume*0.5f; 


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
<<<<<<< HEAD
        stomachVolume = settings.MaxStomachVolume*0.5f;
        currentFeedingState = FeedingState.Normal;
=======

>>>>>>> e01b22a3fc482e5a2129a44dd6f442cfd5587fb8
    }

    Vector3 calculateVcage(Vector3 currentPosition) {
            Vcage = Vector3.zero;
            Vector3 origo = new Vector3(0.0f, currentPosition.y, 0.0f);
            float distance = Vector3.Distance(origo, currentPosition);
            if (distance >= settings.FarmRadius/2-settings.PreferredCageDistance) {
                    Vcage = - (currentPosition - origo);
                }
            if (currentPosition.y >= settings.FarmHeight-settings.PreferredCageDistance) {
                Vcage += new Vector3(0, settings.FarmHeight-settings.PreferredCageDistance-currentPosition.y, 0);
            }
            if (currentPosition.y <= settings.PreferredCageDistance){
                Vcage += new Vector3(0, settings.PreferredCageDistance-currentPosition.y, 0);
            }
            return Vcage;
        }
    public void UpdateFish () {

        //Vcage
        Vector3 currentposition = transform.position;        

        //Feeding stuff
        FeedBehaviour(currentPosition, feedingPosition);

        // Light stuff

        I = Izero * (float)(Math.Exp(0.07f * (currentPosition.y - settings.FarmHeight)));

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
<<<<<<< HEAD

        if(currentFeedingState==FeedingState.Approach ||currentFeedingState==FeedingState.Manipulate)
        {
            Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(Vcage*settings.CageWeight + VFeeding*settings.FeedingWeight + Vso*settings.SocialWeight);
        } else
        {
            Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(Vcage*settings.CageWeight + Vso*settings.SocialWeight + Vli*settings.LightWeight + Vtemp*settings.TempWeight);
        }        
=======
        
        
        Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(calculateVcage(currentposition)*settings.CageWeight + Vso*settings.SocialWeight); // + Vli*settings.LightWeight + Vtemp*settings.TempWeight);
>>>>>>> e01b22a3fc482e5a2129a44dd6f442cfd5587fb8
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
        
        transform.position += Vref * Time.deltaTime;// *Speed;
        transform.rotation = Quaternion.LookRotation(Vref, Vector3.up);
        transform.Rotate(0, 90, 0);
        Vprev = Vref;
    }

    void FeedBehaviour(Vector3 currentPosition, Vector3 feedingPosition){

        
        if(feeding.isFeeding)
        {
            //in Mode Normal -> Satiated
            if(currentFeedingState==FeedingState.Normal && ProbFoodDetection() < 0.75f)
            {
                currentFeedingState = FeedingState.Satiated;
            }
            //in Mode Normal -> Approach            
            else if(currentFeedingState==FeedingState.Normal && ProbFeelingHungry() > 0.75f)
            {   
                currentFeedingState = FeedingState.Approach;
            }
            //in Mode Satiated -> Approach
            else if(currentFeedingState == FeedingState.Satiated && ProbFeelingHungry()>=0.75f)
            {
                currentFeedingState = FeedingState.Approach;
            }
            //in Mode Approach -> Manipulate
            else if(currentFeedingState == FeedingState.Approach && ProbPelletCapture()>=0.75f)
            {
                currentFeedingState = FeedingState.Manipulate;
            }
            //in Mode Manipulate -> Approach
            else if(currentFeedingState == FeedingState.Manipulate && ProbFeelingHungry()>=0.75f)
            {
                currentFeedingState = FeedingState.Approach;
            }
            //in Mode Manipulate -> Satiated
            else if(currentFeedingState == FeedingState.Manipulate && ProbFeelingHungry()<0.75f)
            {
                currentFeedingState = FeedingState.Satiated;
            }
        } 

        switch(currentFeedingState)
        {
            case FeedingState.Normal:
                //continue normally
                VFeeding = Vector3.zero;
                break;
            case FeedingState.Satiated:
                //continue normally
                VFeeding = Vector3.zero;
                break;
            case FeedingState.Approach:
                //move towards feeding area
                //disregard vert axis by temp & light
                VFeeding = feedingPosition - currentPosition;
                break;
            case FeedingState.Manipulate:
                //if pellet capture success -> hold manipulate for between 5-30 sec
                //during manipulate the fish don't respond to temp&light
                //evaluate hunger status after pellet consumption
                VFeeding = Vector3.zero;
                float randTimer = UnityEngine.Random.Range(5f, 30f);
                randTimer -= Time.deltaTime;
                if(randTimer<0.0f){
                    currentFeedingState=FeedingState.Manipulate;
                }           
                break;
            }
    }


    //prob increases when distance to feeding area decreases
    float ProbFoodDetection()
    {
        return 1/(1+Vector3.Distance(feeding.transform.position, transform.position));
    }

    float ProbFeelingHungry()
    {
        float probFeelHunger;
        float rand = UnityEngine.Random.Range(0.0096f, 120f) ;
        float Xnorm = (rand - 0.0096f)/(120f - 0.0096f); // normalised stomach volume

        if (Xnorm >= 0.3){
            probFeelHunger = 0.5f - (0.57f * (Xnorm - 0.3f/ Xnorm -0.2f));
            return probFeelHunger;
        } else {
            probFeelHunger = 0.5f + (0.67f * (0.3f - Xnorm/ 0.4f - Xnorm));
            return probFeelHunger;
        }
    }   

    float ProbPelletCapture()
    {   
        Vector3 xzFeedingPos = new Vector3(feeding.transform.position.x, transform.position.y, feeding.transform.position.z);
        float radius = feeding.transform.localScale.x/10f;
        bool isFishInsideFeedingRadius = Vector3.Distance(xzFeedingPos, transform.position)<radius;

        if (isFishInsideFeedingRadius){
            float probPelletCapture = 0.05f/Mathf.Pow(1+Vector3.Distance(feedingPos, transform.position), 3f);
            return probPelletCapture;
        }else{
            return 0f;
        }
    }
}

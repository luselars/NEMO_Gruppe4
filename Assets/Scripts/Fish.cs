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

    private float stomachVolume;
    private float probFoodDetection;
    private Vector3 feedingPos;
    private Vector3 xzPosFeedingObj;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        feeding = FindObjectOfType<Feeding>();
        
    }

    public void Initialize (FishSettings settings) {
        this.settings = settings;
        Speed = settings.Speed;
        Bodylength = settings.BodyLength;
        stomachVolume = settings.MaxStomachVolume*0.5f;
        
        //velocity = transform.forward * startSpeed;
       
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

        if (transform.position.y >= 9) {
            Vcage += new Vector3(0, 9-transform.position.y, 0);
        }
        if (transform.position.y <= 1){
            Vcage += new Vector3(0, 1.0f-transform.position.y, 0);
        }

        //update position
        Vref = Vprev*settings.DirectionchangeWeight + (1.0f-settings.DirectionchangeWeight)*(Vcage*settings.CageWeight + Vso*settings.SocialWeight);
        Vref = Vref.normalized;

        // Angle updates
        Vector3 VprevHor = new Vector3(Vprev.x, 0, Vprev.z);
        Vector3 VrefHor = new Vector3(Vref.x, 0, Vref.z);

        float HAngle = Vector3.Angle(VprevHor, VrefHor);

        Vref = Vref * Speed;
        if (Vref == Vector3.zero)
        {
            Vref.x = 1;
        }

        float maxY = Speed / 2;
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

    //prob increases when distance to feeding area decreases
    private float ProbFoodDetection()
    {
        return 1/(1+Vector3.Distance(feeding.transform.position, transform.position));
    }    

    private float ProbFeelingHungry()
    {
        float probFeelHunger;
        float rand = Random.Range(0.0096f, 120f) ;
        float Xnorm = (rand - 0.0096f)/(120f - 0.0096f); // normalised stomach volume

        if (Xnorm >= 0.3){
            probFeelHunger = 0.5f - (0.57f * (Xnorm - 0.3f/ Xnorm -0.2f));
            return probFeelHunger;
        } else {
            probFeelHunger = 0.5f + (0.67f * (0.3f - Xnorm/ 0.4f - Xnorm));
            return probFeelHunger;
        }
    }   

    private float ProbPelletCapture()
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

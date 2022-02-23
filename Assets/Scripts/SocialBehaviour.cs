using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialBehaviour : MonoBehaviour
{

    public List<GameObject> Children;
    public float PreferredDist;
    public float Detection;

// Start is called before the first frame update
void Start()
    {
        
         /*foreach (Transform child in transform)
         {
             if (child.tag == "Fish")
             {
                 Children.Add(child.gameObject);
                
             }
         }*/

        PreferredDist = (float)(0.66 * 0.1);
        Detection = (float)(3 * 0.1);
        
    }

    // Update is called once per frame
    void Update()
    {


        for (int i = 0; i < Children.Count; i++)
        {
            for(int j = i; j < Children.Count; j++)
            {
                float dist = Vector3.Distance(Children[i].transform.position, Children[j].transform.position);
               

                
                

                // Makes the fish move 
                if (dist >= PreferredDist && dist < Detection && dist > 0)
                {
                    float mult = (dist - Detection) / (Detection - PreferredDist);
                    Vector3 Nv = (float)0.5 * Children[i].GetComponent<Fish>().Vref * mult;
                    Children[i].GetComponent<Fish>().Sref = (Children[i].GetComponent<Fish>().Sref + Nv);
                    Children[i].GetComponent<Fish>().OtherFish++;
                } else if (dist < PreferredDist && dist > 0) // makes the fish move away if too close
                {
                    Vector3 distVector = Children[j].transform.position - Children[i].transform.position;
                    float mult = (PreferredDist - dist);
                    distVector = distVector * mult;
                    Children[i].GetComponent<Fish>().OtherFish++;
                    //Children[i].GetComponent<Fish>().Sref = (Children[i].GetComponent<Fish>().Sref + distVector);
                }
            }
        }


        
    }
}

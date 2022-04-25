using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCameraPos : MonoBehaviour
{
    
    //values that will be set in the Inspector
    public Transform Target;
    private float RotationSpeed = 1.0f;

     
    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;

    public GameObject camera;
    public FishSettings settings;

    void Start()
    {
        camera.transform.position = new Vector3(camera.transform.position.x + settings.FarmRadius + 2, settings.FarmHeight - 4, camera.transform.position.z - 2);
    }

    // Update is called once per frame
    void Update()
    {
        //find the vector pointing from our position to the target
        _direction = (Target.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    }
}

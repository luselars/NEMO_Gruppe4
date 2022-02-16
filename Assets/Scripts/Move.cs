using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    public Transform target;
    public float speed = 1.0f;

    private Vector3 RandomVector(float min, float max)
    {
        var x = Random.Range(min, max);
        var y = Random.Range(min, max);
        var z = Random.Range(min, max);
        return new Vector3(x, y, z);
    }
    // Start is called before the first frame update
    void Awake()
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    void Update()
    {

        var rb = GetComponent<Rigidbody>();
        Vector3 movementVector = (RandomVector(0f, 5f) + rb.velocity);
        rb.velocity = RandomVector(0f, 5f);
    }

}

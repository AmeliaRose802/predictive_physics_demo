using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        rb.AddForceAtPosition(transform.forward * speed, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

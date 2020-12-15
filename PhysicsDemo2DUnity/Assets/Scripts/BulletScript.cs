using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Simple script to move bullets foward and mark their path
 */
public class BulletScript : MonoBehaviour
{
    public GameObject trajIndicator;
    public float force = 10;
    float trajTimeToLive = 3f;

    Rigidbody2D rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * force;
        Destroy(this, 10f);

        StartCoroutine(TraceAtInterval());
    }

    
    IEnumerator TraceAtInterval()
    {
        
        for(int i = 0; i < 30; i++)
        {
            GameObject traj = Instantiate(trajIndicator, transform.position, transform.rotation);
            Destroy(traj, trajTimeToLive);
            yield return new WaitForSeconds(.05f);
        }
    }

    
}

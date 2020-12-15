using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DogeScript : MonoBehaviour
{
    public Material hitMaterial;
    public Material missMaterial;
    public Material impactMaterial;
    public SphereCollider collider;
    public Rigidbody myRigidBody;
    float speed = 500;
    private float fixedDeltaTime;

    const float g = 9.8f; //TODO: Should probly be pulled from settings not defined here

    private void Start()
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
        myRigidBody = GetComponent<Rigidbody>();
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    private void OnCollisionEnter(Collision collision)
    {
     if(collision.gameObject.tag == "Bullet")
        {
            gameObject.GetComponent<MeshRenderer>().material = missMaterial;
            myRigidBody.useGravity = true;
        }   
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Debug.Log("Detected incoming bullet");
             checkForInterception(other.gameObject);

            //findXIntersect(other.gameObject);
            
        }
    }

    private void findXIntersect(GameObject hitBy)
    {
        Rigidbody rb = hitBy.GetComponent<Rigidbody>();
        float xDistance = Mathf.Abs(hitBy.transform.position.x - transform.position.x);
        float t = xDistance / Mathf.Abs(rb.velocity.x);
        
        
        
        float zDistance = Mathf.Abs(hitBy.transform.position.z - transform.position.z);
        float tz= zDistance / Mathf.Abs(rb.velocity.z);
        
        
        if (t < 0 || tz <0)
        {
            return;
        }

        Debug.Log("Could actually hit in in a positive amount of time");
        Debug.Log("Predicted based on z " + tz);
        Debug.Log("Predected time based on X: " + t);

        float yPos = hitBy.transform.position.y + rb.velocity.y * tz * .5f * g * rb.velocity.y * rb.velocity.y;
        Vector3 projectedHit = new Vector3(hitBy.transform.position.x + tz * rb.velocity.x, hitBy.transform.position.z + yPos, tz * rb.velocity.z);

        Debug.Log((transform.position - projectedHit).magnitude);
        if((transform.position - projectedHit).magnitude < collider.radius)
        {
            hitBy.GetComponent<MeshRenderer>().material = hitMaterial;
        }
       


    }
    //Returns a vector 2 with both solutions. If no solution exists returns a vector2 containing -1 in both terms
    private Vector3 checkForInterception(GameObject hitBy)
    {
        Rigidbody rb = hitBy.GetComponent<Rigidbody>();

        float radius = collider.radius;

        float v0 = rb.velocity.y;


        float bulletRadius = hitBy.GetComponent<SphereCollider>().radius * hitBy.transform.localScale.y;

        for(float y = transform.position.y - radius; y < transform.position.y + radius; y+= bulletRadius)
        {
            

            float t = getTimeAtYA(v0, y);
            

            checkIfAnswer(t, rb, hitBy);

            float t2 = getTimeAtYB(v0, y);

            checkIfAnswer(t2, rb, hitBy);
        }


        return Vector3.zero;
    }

    void checkIfAnswer(float t, Rigidbody rb, GameObject hitBy)
    {
        if (isValid(t))
        {
            float xPos = getPosAtTime(rb.velocity.x, t, hitBy.transform.position.x);
            
            if (isXWithinRadius(xPos))
            {
                float zPos = getPosAtTime(rb.velocity.z, t, hitBy.transform.position.z);


                if (isZWithinRadius(zPos))
                {
                    hitBy.GetComponent<MeshRenderer>().material = hitMaterial;
                    Vector3 impactPos = new Vector3(xPos, transform.position.y, zPos);
                    Debug.Log("V0" + rb.velocity);
                    Debug.Log("Innital Position" + hitBy.transform.position);
                    Debug.Log("Projected Position" + impactPos);
                    Debug.Log("Target Position " + transform.position);
                    Debug.Log("Time: " + t);
                    fleeBullet(rb.velocity);
                }
            }
        }

    }
    float getTimeAtYA(float v0, float yPos)
    {
        float sqrtTermA = v0 * v0 - 2 * g * (transform.position.y - yPos);
        float t = Mathf.Sqrt(-v0 + Mathf.Sqrt(sqrtTermA) / g);

        return t;
    }

    float getTimeAtYB(float v0, float yPos)
    {
        float sqrtTermA = v0 * v0 - 2 * g * (transform.position.y - yPos);
        float t = Mathf.Sqrt(-v0 - Mathf.Sqrt(sqrtTermA) / g);

        return t;
    }

    float getPosAtTime(float vel, float time, float start)
    {
        return start + vel * time;
    }

    bool isXWithinRadius(float xVal)
    {
        return Mathf.Abs(transform.position.x - xVal) < collider.radius * 1.5 * transform.localScale.x;
    }

    bool isZWithinRadius(float zVal)
    {
        return Mathf.Abs(transform.position.z - zVal) < collider.radius * 1.5 * transform.localScale.x;
    }
    bool checkCollision(Vector3 point)
    {
        return (transform.position - point).magnitude < (collider.radius* transform.localScale.x * 2);
    }

    bool isValid(float t)
    {
        if (float.IsNaN(t))
        {
            return false;
        }
        else if(t < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void fleeBullet(Vector3 bulletVelocity)
    {
        
        Vector3 forceVector = Random.onUnitSphere;

        if(bulletVelocity.x > bulletVelocity.y && bulletVelocity.x > bulletVelocity.z)
        {
            forceVector.y *= 2;
            forceVector.z *= 2;
        }
        if (bulletVelocity.y > bulletVelocity.x && bulletVelocity.y > bulletVelocity.z)
        {
            forceVector.x *= 2;
            forceVector.z *= 2;
        }
        if (bulletVelocity.z > bulletVelocity.y && bulletVelocity.z > bulletVelocity.x)
        {
            forceVector.y *= 2;
            forceVector.x *= 2;
        }

        ;
        myRigidBody.AddForce(forceVector.normalized * speed);
        

        
        StartCoroutine(stopMoving());
    }

   IEnumerator stopMoving()
    {
        yield return new WaitForSeconds(1f);
        myRigidBody.velocity = Vector3.zero;
        Destroy(this.gameObject);
    }
}

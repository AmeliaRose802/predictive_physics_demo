using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Predict the path of incoming projectiles and get out of the way
 */
public class DodgeScript : MonoBehaviour
{
    public GameObject hitMarker;

    //Private Vars
    const float g = 9.8f;
    float enemyRadius;
    float maxSpeed = 10f;
    Rigidbody2D enemyRb;
    bool returningHome = false;
    Vector3 home;

    private void Start()
    {
        enemyRadius = (GetComponent<CircleCollider2D>().radius * transform.localScale.y * 1.2f);
        enemyRb = GetComponent<Rigidbody2D>();
        home = transform.position;
    }

    private void Update()
    {

        //When doging return to innital position after set interval
        if (returningHome)
        {
            float step = maxSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, home, step);
            if(Vector2.SqrMagnitude(transform.position - home) < 0.0001)
            {
                returningHome = false;
            }
        }    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(this.gameObject);
        }
        StartCoroutine(ReturnHome());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Bullet"))
        {
            CalcBulletTrajectory(other.gameObject);
        }
    }

    void CalcBulletTrajectory(GameObject bullet)
    {
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 distance =  transform.position - bullet.transform.position;
        
        //Find time to reach position target
        float timeToTarget = distance.x / rb.velocity.x;
        
        //Since time can't be negitive an answer less then 0 indicates that no solution exists
        if(timeToTarget < 0)
        {
            return;
        }

        //Calculate projected position of bullet after time
        float y = bullet.transform.position.y + (-.5f * g * timeToTarget * timeToTarget) + (rb.velocity.y * timeToTarget);
        float x = bullet.transform.position.x + timeToTarget * rb.velocity.x;
        
        //Create a red marker for the hit point
        GameObject newHitMarker = Instantiate(hitMarker, new Vector3(x, y, bullet.transform.position.z), transform.rotation);
        Destroy(newHitMarker, 2f);

        //See if it is a hit
        

        if(Mathf.Abs(transform.position.y - y) < enemyRadius)
        {
            Vector2 projectedVelocity = rb.velocity - new Vector2(0, g) * timeToTarget;
            Flee(new Vector2(x, y), projectedVelocity);

        }

    }

    //Use any flee steering you want. This simply calculates a direction to move perpindicular to the incoming bullet
    void Flee(Vector2 hitPoint, Vector2 bulletVelocity)
    {
        
        //https://gamedev.stackexchange.com/questions/70075/how-can-i-find-the-perpendicular-to-a-2d-vector
        Vector2 perp;

        //Move up or down based on what y position it is supposed to hit
        if(hitPoint.y > transform.position.y)
        {
            perp = new Vector2(bulletVelocity.y, -bulletVelocity.x);
        }
        else
        {
            perp = new Vector2(-bulletVelocity.y, bulletVelocity.x);
        }

        perp = perp.normalized * maxSpeed;

        enemyRb.velocity = perp;

        //Return home after a few seconds
        returningHome = false;
        StartCoroutine(ReturnHome());
    }

    IEnumerator ReturnHome()
    {
        yield return new WaitForSeconds(.5f);
        returningHome = true;
        enemyRb.velocity = Vector2.zero;
    }
    
}

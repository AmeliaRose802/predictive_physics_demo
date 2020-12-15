using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Very quick and dirty move and shoot script
 */
public class MoveScript : MonoBehaviour
{
    public float speed = 5;
    public float rotSpeed = 50;
    public GameObject bullet;
    public Transform shootPoint;

    

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float y =  vertical * speed * Time.deltaTime;
        float x = ( horizontal * speed * Time.deltaTime);

        //Uncomment to rotate instead of moving on the x axis
        //transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + x);

        transform.position += new Vector3(x, y, 0);

        //Fire!
        if (Input.GetKeyDown("space"))
        {
            Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        }
    }
}

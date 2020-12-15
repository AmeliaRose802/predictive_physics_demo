using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public GameObject bullet;
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject theBullet = Instantiate(bullet, transform.position, playerCamera.transform.rotation);
            //Destroy(theBullet, 5);
            theBullet.transform.position = playerCamera.transform.position + playerCamera.transform.forward;

        }
    }
}

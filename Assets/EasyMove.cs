using UnityEngine;
using System.Collections;

public class EasyMove : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        float value = Input.GetAxis("Horizontal");
        body.AddTorque(0, value*400, 0);

        bool isThrustActive = Input.GetButton("Fire1");
        if (isThrustActive)
            body.AddForce(body.transform.forward*400);
    }
}

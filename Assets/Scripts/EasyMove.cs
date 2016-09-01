﻿using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float TORQUE_MAX = 200000; //Newtons
    private const float THRUST_MAX = 200000; //Newtons
    private const float THRUST_VECTORING_ANGLE_MAX = 20;  //°

    private float nozzleAngle;
    private bool isThrustActive;

    public Transform CenterOfMass;
    public Transform ThrusterPivot;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition;
    }

	void Update()
	{
        this.nozzleAngle = Input.GetAxis("Horizontal");
        this.isThrustActive = Input.GetButton("Fire1");
	}
	
	void FixedUpdate()
	{
		Rigidbody rigidbody = GetComponent<Rigidbody>();

        rigidbody.AddRelativeTorque(Vector3.up * nozzleAngle * TORQUE_MAX);

	    if (isThrustActive)
	    {
            Quaternion thrustAngle = Quaternion.AngleAxis(-nozzleAngle * THRUST_VECTORING_ANGLE_MAX, rigidbody.transform.up);
            Vector3 thrust = thrustAngle * rigidbody.transform.forward * THRUST_MAX;
            rigidbody.AddForceAtPosition(thrust, ThrusterPivot.position);
	    }
	}
}

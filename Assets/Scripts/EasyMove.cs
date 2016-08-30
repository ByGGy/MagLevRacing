using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float THRUST_MAX = 156000; //Newtons

    private float nozzleAngle;
    private bool isThrustActive;

	void Update()
	{
        this.nozzleAngle = Input.GetAxis("Horizontal");
        this.isThrustActive = Input.GetButton("Fire1");
	}
	
	void FixedUpdate()
	{
		Rigidbody rigidbody = GetComponent<Rigidbody>();

        rigidbody.AddTorque(0, nozzleAngle * THRUST_MAX*2, 0);

		if (isThrustActive)
		    rigidbody.AddForce(rigidbody.transform.forward * THRUST_MAX);
	}
}

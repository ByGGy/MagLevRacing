using UnityEngine;

public class EasyMove : MonoBehaviour
{
#error Need a reference to following Empty GameObjects :
        - Center of Mass
        - Thruster Pivot

    private const float THRUST_MAX = 156000; //Newtons
    private const float THRUST_VECTORING_ANGLE_MAX = 20;  //°

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

        //rigidbody.AddTorque(0, nozzleAngle * THRUST_MAX*2, 0);

	    if (isThrustActive)
	    {
            Quaternion thrustAngle = Quaternion.AngleAxis(nozzleAngle * THRUST_VECTORING_MAX, rigidbody.transform.up);
            Vector3 thrust = thrustAngle * rigidbody.transform.forward * THRUST_MAX;
            rigidbody.AddForceAtPosition(thrust, rigidbody.transform.position - rigidbody.transform.forward*9);
	    }
	}
}

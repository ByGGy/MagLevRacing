using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float TORQUE_MAX = 180000; //Newtons
    private const float THRUST_MAX = 180000; //Newtons
    private const float THRUST_VECTORING_ANGLE_MAX = 30;  //°

    private float nozzleAngle;
    private bool isThrustActive;

    public Transform CenterOfMass;
    public Transform ThrusterPivot;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition;
    }

    private void Update()
	{
        this.nozzleAngle = Input.GetAxis("Yaw");
        this.isThrustActive = Input.GetButton("Accelerate");

        Transform nozzle = this.transform.FindChild("Nozzle");
        nozzle.transform.localRotation = Quaternion.AngleAxis(-nozzleAngle * THRUST_VECTORING_ANGLE_MAX, nozzle.transform.up);

        ParticleSystem smoke = nozzle.FindChild("Smoke").GetComponent<ParticleSystem>();
        var doppleGangerEmission = smoke.emission;
        doppleGangerEmission.rate = new ParticleSystem.MinMaxCurve(isThrustActive ? 400 : 0);
	}

    private void FixedUpdate()
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

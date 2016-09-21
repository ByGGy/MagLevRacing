using System.Linq;
using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float TORQUE_MAX = 180000; //Newtons
    private const float THRUST_MAX = 180000; //Newtons
    private const float THRUST_VECTORING_ANGLE_MAX = 30;  //°

    private float nozzleAngle;
    private bool isThrustActive;

    private AudioSource engineInSample;
    private AudioSource engineOutSample;

    public Transform CenterOfMass;
    public Transform ThrusterPivot;

    private void Start()
    {
        this.engineInSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "EngineInSample"));
        this.engineOutSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "EngineOutSample"));

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
        doppleGangerEmission.rate = new ParticleSystem.MinMaxCurve(isThrustActive ? 400 : 10);


        if (this.isThrustActive)
        {
            this.engineInSample.volume = Mathf.Lerp(this.engineInSample.volume, 0.3f, Time.deltaTime * 2);
            this.engineInSample.pitch = Mathf.Lerp(this.engineInSample.pitch, 1.25f, Time.deltaTime * 2);
            this.engineOutSample.volume = Mathf.Lerp(this.engineOutSample.volume, 1.0f, Time.deltaTime * 2);
            this.engineOutSample.pitch = Mathf.Lerp(this.engineOutSample.pitch, 1.1f, Time.deltaTime * 2);
        }
        else
        {
            this.engineInSample.volume = Mathf.Lerp(this.engineInSample.volume, 0.2f, Time.deltaTime * 3);
            this.engineInSample.pitch = Mathf.Lerp(this.engineInSample.pitch, 0.75f, Time.deltaTime * 3);
            this.engineOutSample.volume = Mathf.Lerp(this.engineOutSample.volume, 0.1f, Time.deltaTime * 3);
            this.engineOutSample.pitch = Mathf.Lerp(this.engineOutSample.pitch, 0.75f, Time.deltaTime * 3);
        }

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

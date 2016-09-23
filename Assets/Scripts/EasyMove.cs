using System.Linq;
using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float MAGLEV_RADIUS = 5.0f;
    private const int MAGLEV_SAMPLES = 12;

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

        //MagLev
        float maxMagnitude = GetComponent<Rigidbody>().mass * 9.81f;
        Vector3 magLevForcePerSample = this.transform.up * maxMagnitude;

        for (int i = 0; i < MAGLEV_SAMPLES; i++)
        {
            RaycastHit hit;
            float offset = i * 2 * Mathf.PI / MAGLEV_SAMPLES;
            Vector3 samplePosition = this.CenterOfMass.position + MAGLEV_RADIUS * Mathf.Cos(offset) * this.transform.forward + MAGLEV_RADIUS * Mathf.Sin(offset) * this.transform.right;
            if (Physics.Raycast(samplePosition, -this.transform.up, out hit, 10))
            {
                float cosine = Vector3.Dot(hit.normal, this.transform.up);
                if (cosine > 0)
                    rigidbody.AddForceAtPosition((magLevForcePerSample * cosine * cosine) / Mathf.Exp(hit.distance), samplePosition);
            }
        }

        //Thruster
        rigidbody.AddRelativeTorque(Vector3.up * nozzleAngle * TORQUE_MAX);

	    if (isThrustActive)
	    {
            Quaternion thrustAngle = Quaternion.AngleAxis(-nozzleAngle * THRUST_VECTORING_ANGLE_MAX, rigidbody.transform.up);
            Vector3 thrust = thrustAngle * rigidbody.transform.forward * THRUST_MAX;
            rigidbody.AddForceAtPosition(thrust, ThrusterPivot.position);
	    }

        //Gravity
        //Vector3 gravity = Vector3.down * GetComponent<Rigidbody>().mass * 9.81f;
        //rigidbody.AddForceAtPosition(gravity, this.CenterOfMass.position);
	}

    private void LateUpdate()
    {
        float maxMagnitude = GetComponent<Rigidbody>().mass * 9.81f;
        Vector3 magLevForcePerSample = this.transform.up * maxMagnitude;

        for (int i = 0; i < MAGLEV_SAMPLES; i++)
        {
            RaycastHit hit;
            float offset = i * 2 * Mathf.PI / MAGLEV_SAMPLES;
            Vector3 samplePosition = this.CenterOfMass.position + MAGLEV_RADIUS * Mathf.Cos(offset) * this.transform.forward + MAGLEV_RADIUS * Mathf.Sin(offset) * this.transform.right;
            if (Physics.Raycast(samplePosition, -this.transform.up, out hit, 10))
            {
                float cosine = Vector3.Dot(hit.normal, this.transform.up);
                if (cosine > 0)
                {
                    Debug.DrawRay(samplePosition, (magLevForcePerSample * cosine * cosine) / Mathf.Exp(hit.distance) / 500, Color.green, 0, false);
                }
            }
        }        
    }
}

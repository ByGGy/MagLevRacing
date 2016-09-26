using System.Linq;
using UnityEngine;

public class EasyMove : MonoBehaviour
{
    private const float PHYSX_CONTROL_DRAG = 0.3f;
    private const float PHYSX_CONTROL_ANGULARDRAG = 2f;

    private const float PHYSX_FREEFALL_DRAG = 0.001f;
    private const float PHYSX_FREEFALL_ANGULARDRAG = 1f;

    private const float MAGLEV_MAXHEIGHT = 4f;
    private const float MAGLEV_RADIUS = 6.0f;
    private const int MAGLEV_SAMPLES = 8;

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
        GetComponent<Rigidbody>().drag = PHYSX_FREEFALL_DRAG;
        GetComponent<Rigidbody>().angularDrag = PHYSX_FREEFALL_ANGULARDRAG;
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
        float maxMagnitudePerSample = GetComponent<Rigidbody>().mass * 9.81f / MAGLEV_SAMPLES;

        int sampleCount = 0;
        for (int i = 0; i < MAGLEV_SAMPLES; i++)
        {
            RaycastHit hit;
            float offset = i * 2 * Mathf.PI / MAGLEV_SAMPLES;
            Vector3 samplePosition = this.CenterOfMass.position + MAGLEV_RADIUS * Mathf.Cos(offset) * this.transform.forward + MAGLEV_RADIUS * Mathf.Sin(offset) * this.transform.right;
            Vector3 magLevForceDirection = this.transform.up.normalized;
            if (Physics.Raycast(samplePosition, -magLevForceDirection, out hit, MAGLEV_MAXHEIGHT))
            {
                float cosine = Vector3.Dot(hit.normal, magLevForceDirection);
                
                float anotherCosine = Vector3.Dot(hit.normal, -GetComponent<Rigidbody>().velocity.normalized);
                float antiImpact = 1 + anotherCosine*anotherCosine;// * GetComponent<Rigidbody>().velocity.magnitude;
                //if (cosine > 0.25f)
                {
                    //Efficiency of 1 at MAGLEV_HEIGHT / 2
                    //float efficiency = cosine * cosine / Mathf.Exp(hit.distance);
                    float efficiency = Mathf.Exp(MAGLEV_MAXHEIGHT * 0.5f) / Mathf.Exp(hit.distance);
                    rigidbody.AddForceAtPosition(hit.normal * maxMagnitudePerSample * efficiency * antiImpact, samplePosition);
                    Debug.DrawRay(samplePosition, hit.normal * maxMagnitudePerSample * efficiency * antiImpact / 500, Color.cyan, 0, false);
                    sampleCount++;
                }
            }
        }

        //Drag
        bool isFreeFall = sampleCount < MAGLEV_SAMPLES * 0.5f;
        GetComponent<Rigidbody>().drag = isFreeFall ? PHYSX_FREEFALL_DRAG : PHYSX_CONTROL_DRAG;
        GetComponent<Rigidbody>().angularDrag = isFreeFall ? PHYSX_FREEFALL_ANGULARDRAG : PHYSX_CONTROL_ANGULARDRAG;

        //Thruster
        rigidbody.AddRelativeTorque(Vector3.up * nozzleAngle * TORQUE_MAX);

	    if (isThrustActive)
	    {
            Quaternion thrustAngle = Quaternion.AngleAxis(-nozzleAngle * THRUST_VECTORING_ANGLE_MAX, rigidbody.transform.up);
            Vector3 thrust = thrustAngle * rigidbody.transform.forward * THRUST_MAX;
            rigidbody.AddForceAtPosition(thrust, ThrusterPivot.position);
	    }
	}
}

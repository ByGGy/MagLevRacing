using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagLevSample
{
    public readonly Vector3 Position;
    public readonly Vector3 Direction;
    public readonly float Weight;

    public static List<MagLevSample> Generate(int qty, Vector3 center, float radius, Vector3 forward, Vector3 right, Vector3 up, Vector3 velocity)
    {
        Vector3 sampleDirection = forward;
        Quaternion sampleOffset = Quaternion.AngleAxis(360.0f / qty, up);

        return Enumerable.Range(0, qty).Select(index => 
        {
            Vector3 position = center + sampleDirection * radius;

            //Terminal velocity estimated at 110 m/s
            float weightConstantPart = 0;//1 - velocity.magnitude / 110;
            float weightDynamicPart = 1 - weightConstantPart;

            Vector3 vProjected = velocity - Vector3.Project(velocity, up);
            float factor = 1 - Vector3.Angle(sampleDirection, vProjected) / 180.0f;
            factor *= 1.75f;

            sampleDirection = sampleOffset * sampleDirection;
            return new MagLevSample(position, -up, weightConstantPart + weightDynamicPart * factor);
        }).ToList();
    }

    public MagLevSample(Vector3 position, Vector3 direction, float weight)
    {
        this.Position = position;
        this.Direction = direction;
        this.Weight = weight;
    }
}

public class EasyMove : MonoBehaviour
{
    private const float PHYSX_CONTROL_DRAG = 0.3f;
    private const float PHYSX_CONTROL_ANGULARDRAG = 2f;

    private const float PHYSX_FREEFALL_DRAG = 0.001f;
    private const float PHYSX_FREEFALL_ANGULARDRAG = 0.001f;

    private const float MAGLEV_MAXHEIGHT = 4f;
    private const float MAGLEV_RADIUS = 3.0f;
    private const int MAGLEV_SAMPLE_QTY = 8;

    private const float MASS = 5000; //Kg
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

        GetComponent<Rigidbody>().mass = MASS;
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

        //Gravity
        rigidbody.AddForce(-Vector3.up * rigidbody.mass * 9.81f);

        //Drag
        Vector3 vProjected = rigidbody.velocity - Vector3.Project(rigidbody.velocity, Vector3.up);
        rigidbody.AddForce(-vProjected * rigidbody.mass * PHYSX_CONTROL_DRAG);
        rigidbody.angularDrag = PHYSX_CONTROL_ANGULARDRAG;

        //MagLev
        float maxMagnitudePerSample = rigidbody.mass * 9.81f / MAGLEV_SAMPLE_QTY;

        List<MagLevSample> samples = MagLevSample.Generate(MAGLEV_SAMPLE_QTY, this.CenterOfMass.position, MAGLEV_RADIUS, this.transform.forward, this.transform.right, this.transform.up, rigidbody.velocity);

        int hitQty = 0;
        samples.ForEach(s =>
        {
            Debug.DrawRay(this.CenterOfMass.position, (s.Position - this.CenterOfMass.position).normalized * s.Weight * 5, Color.green, 0, false);

            RaycastHit hit;
            if (Physics.Raycast(s.Position, s.Direction, out hit, MAGLEV_MAXHEIGHT))
            {
                float efficiency = Math.Min(Mathf.Exp(MAGLEV_MAXHEIGHT * 0.5f) / Mathf.Exp(hit.distance), 4);
                rigidbody.AddForceAtPosition(hit.normal * maxMagnitudePerSample * efficiency * s.Weight, s.Position);

                Debug.DrawRay(s.Position, hit.normal * efficiency * s.Weight * 5, Color.cyan, 0, false);

                hitQty++;
            }
        });

        /*
        for (int i = 0; i < MAGLEV_SAMPLE_QTY; i++)
        {
            RaycastHit hit;
            float offset = i * 2 * Mathf.PI / MAGLEV_SAMPLE_QTY;
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
                    hitQty++;
                }
            }
        }*/

        //Drag
//        bool isFreeFall = hitQty < MAGLEV_SAMPLE_QTY * 0.5f;
//        float angle = Vector3.Angle(this.transform.forward, GetComponent<Rigidbody>().velocity);
//        GetComponent<Rigidbody>().drag = PHYSX_CONTROL_DRAG * angle / 180.0f;
//        GetComponent<Rigidbody>().angularDrag = PHYSX_CONTROL_ANGULARDRAG * angle / 180.0f;
//        GetComponent<Rigidbody>().drag = isFreeFall ? PHYSX_FREEFALL_DRAG : PHYSX_CONTROL_DRAG;
//        GetComponent<Rigidbody>().angularDrag = isFreeFall ? PHYSX_FREEFALL_ANGULARDRAG : PHYSX_CONTROL_ANGULARDRAG;

        //Fake Orientation Thruster
        if (hitQty > 0)
            rigidbody.AddRelativeTorque(Vector3.up * nozzleAngle * TORQUE_MAX);

        //Main Thruster
	    if (isThrustActive)
	    {
            Quaternion thrustAngle = Quaternion.AngleAxis(-nozzleAngle * THRUST_VECTORING_ANGLE_MAX, rigidbody.transform.up);
            Vector3 thrust = thrustAngle * rigidbody.transform.forward * THRUST_MAX;
            rigidbody.AddForceAtPosition(thrust, ThrusterPivot.position);
	    }
	}
}

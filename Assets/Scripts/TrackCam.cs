using UnityEngine;
using System.Collections;

public class TrackCam : MonoBehaviour
{
	public Transform Target;
	
	void LateUpdate()
	{
	    Vector3 offset = Target.up - Target.forward  - Target.GetComponent<Rigidbody>().velocity.normalized;
        offset.Normalize();
        transform.position = Target.position + offset * (40 + Target.GetComponent<Rigidbody>().velocity.magnitude * 0.5f);
		transform.LookAt(Target);
	}
}

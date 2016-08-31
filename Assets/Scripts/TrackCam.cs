using UnityEngine;
using System.Collections;

public class TrackCam : MonoBehaviour
{
	public Transform Target;
	
	void LateUpdate()
	{
        transform.position = Target.position - Target.forward * 40 + Target.up * 40 - Target.GetComponent<Rigidbody>().velocity;
		transform.LookAt(Target);
	}
}

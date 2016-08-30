using UnityEngine;
using System.Collections;

public class TrackCam : MonoBehaviour
{
	public Transform TargetTransform;
	
	void LateUpdate()
	{
        transform.position = TargetTransform.position - TargetTransform.forward * 40 - TargetTransform.GetComponent<Rigidbody>().velocity + TargetTransform.up * 40;
		transform.LookAt(TargetTransform);
	}
}

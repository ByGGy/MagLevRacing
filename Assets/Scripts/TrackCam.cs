using UnityEngine;

public class TrackCam : MonoBehaviour
{
	public Transform Target;

    private bool isAerial;

    private void Start()
    {
        this.isAerial = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cycle View"))
            this.isAerial = !this.isAerial;
    }

    private void LateUpdate()
	{
	    Vector3 offset = Target.up - Target.forward;

        float velocity = Target.GetComponent<Rigidbody>().velocity.magnitude;

        if (velocity > Mathf.Epsilon)
	        offset -= Target.GetComponent<Rigidbody>().velocity.normalized;
        else
            offset -= Target.forward;

        offset.Normalize();
        transform.position = Target.position + offset * Mathf.Max(15, Target.GetComponent<Rigidbody>().velocity.magnitude * 0.3f);

        if (this.isAerial)
            transform.position += Target.up * 150;

		transform.LookAt(Target);
	}
}

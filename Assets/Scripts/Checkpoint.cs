using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public uint Id;
    public Collider Target;

    public event Action<Checkpoint> OnTargetDetected;

	private void OnTriggerEnter(Collider other)
	{
		if (this.Target == other)
		{
            if (OnTargetDetected != null)
            	OnTargetDetected.Invoke(this);
		}
	}
}
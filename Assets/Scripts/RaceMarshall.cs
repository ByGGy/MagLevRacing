using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceMarshall : MonoBehaviour
{
    private List<Checkpoint> checkpoints;

	// Use this for initialization
    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);
        Debug.Log(this.checkpoints.Count);
	}

    private void OnCheckpointTriggered(Checkpoint cp)
    {
        Debug.Log(string.Format("Checkpoint {0}", cp.Id));
    }

	// Update is called once per frame
    private void Update()
    {
	
	}
}

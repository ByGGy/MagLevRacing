using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LapIntermediateRecord
{
    public readonly uint CheckpointId;
    public readonly float ElapsedTime;

    public LapIntermediateRecord(uint checkpointId, float elapsedTime)
    {
        this.CheckpointId = checkpointId;
        this.ElapsedTime = elapsedTime;
    }
}

public class RaceMarshall : MonoBehaviour
{
    private List<Checkpoint> checkpoints;

    private bool isRecordingLap;
    private float elapsedTime;
    private List<LapIntermediateRecord> intermediateRecords;

    public Transform Ship;

    public float ElapsedTime { get { return this.elapsedTime; } }
    public IEnumerable<LapIntermediateRecord> IntermediateRecords { get { return this.intermediateRecords; } }

    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().OrderBy(cp => cp.Id).ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);
        
        ResetShip();
        ResetRecording();
	}

    private void OnCheckpointTriggered(Checkpoint cp)
    {
        if (cp.Id == this.checkpoints.First().Id)
        {
            if (!isRecordingLap)
                StartRecordingLap(cp);
            else
                EndRecordingLap(cp);
        }
        else
            RecordLap(cp);
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            ResetShip();
            ResetRecording();
        }
    }

    private void FixedUpdate()
    {
        if (this.isRecordingLap)
            this.elapsedTime += Time.fixedDeltaTime;
    }

    private void ResetShip()
    {
        this.Ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.Ship.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.Ship.position = this.transform.FindChild("StartingBlock").position;
        this.Ship.rotation = this.transform.FindChild("StartingBlock").rotation;
    }

    private void ResetRecording()
    {
        this.isRecordingLap = false;
        this.elapsedTime = 0;
        this.intermediateRecords = new List<LapIntermediateRecord>();
    }

    private void StartRecordingLap(Checkpoint cp)
    {
        this.isRecordingLap = true;
        this.elapsedTime = 0;
        this.intermediateRecords = new List<LapIntermediateRecord>();
        this.intermediateRecords.Add(new LapIntermediateRecord(cp.Id, this.elapsedTime));
    }

    private void RecordLap(Checkpoint cp)
    {
        if (this.isRecordingLap)
        {
            if (cp.Id == this.intermediateRecords.Last().CheckpointId + 1)
                this.intermediateRecords.Add(new LapIntermediateRecord(cp.Id, this.elapsedTime));
        }
    }

    private void EndRecordingLap(Checkpoint cp)
    {
        this.isRecordingLap = false;
        this.intermediateRecords.Add(new LapIntermediateRecord(cp.Id, this.elapsedTime));
        this.elapsedTime = 0;
    }
}
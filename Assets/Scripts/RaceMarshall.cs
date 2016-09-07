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

    public IEnumerable<LapIntermediateRecord> IntermediateRecords { get { return this.intermediateRecords; } }

    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().OrderBy(cp => cp.Id).ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);
        Debug.Log(this.checkpoints.Count);
	}

    private void OnCheckpointTriggered(Checkpoint cp)
    {
        Debug.Log(string.Format("Checkpoint {0}", cp.Id));
        if (cp.Id == this.checkpoints.First().Id)
        {
            if (!isRecordingLap)
                StartRecordingLap(cp);
            else
                EndRecordingLap(cp);
        }
        else
            RecordLap(cp)
    }

    private void FixedUpdate()
    {
        this.elapsedTime += fixedDeltaTime;
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
        if (cp.Id == this.intermediateRecords.Last().CheckpointId + 1)
            this.intermediateRecords.Add(new LapIntermediateRecord(cp.Id, this.elapsedTime));
    }

    private void EndRecordingLap(Checkpoint cp)
    {
        this.isRecordingLap = false;
        this.intermediateRecords.Add(new LapIntermediateRecord(cp.Id, this.elapsedTime));
        Debug.Log(string.Format("Lap Time = {0} ms", this.elapsedTime));
    }
}
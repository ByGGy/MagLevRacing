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

public class LapRecord
{
    private static uint Qty = 0;

    public readonly uint Id;
    private readonly List<LapIntermediateRecord> intermediateRecords;

    public LapRecord()
    {
        this.Id = ++Qty;
        this.intermediateRecords = new List<LapIntermediateRecord>();
    }

    public void AddIntermediate(LapIntermediateRecord record)
    {
        if (this.intermediateRecords.Any())
        {
            if ((record.CheckpointId == this.intermediateRecords.Last().CheckpointId + 1) || (record.CheckpointId == this.intermediateRecords.First().CheckpointId))
                this.intermediateRecords.Add(record);
        }
        else
            this.intermediateRecords.Add(record);
    }

    public bool IsComplete
    {
        get { return (this.intermediateRecords.Count() > 1) && (this.intermediateRecords.First().CheckpointId == this.intermediateRecords.Last().CheckpointId); }
    }

    public float Time
    {
        get { return this.intermediateRecords.Any() ? this.intermediateRecords.Last().ElapsedTime : 0; }
    }

    public IEnumerable<LapIntermediateRecord> IntermediateRecords { get { return this.intermediateRecords; } }
}

public class RaceMarshall : MonoBehaviour
{
    private List<Checkpoint> checkpoints;

    private float elapsedTime;
    private LapRecord currentLapRecord;

    public Transform Ship;

    public float ElapsedTime { get { return this.elapsedTime; } }
    public LapRecord CurrentLapRecord { get { return this.currentLapRecord; } }

    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().OrderBy(cp => cp.Id).ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);

        Reset();
	}

    private void Reset()
    {
        this.elapsedTime = 0;
        this.currentLapRecord = null;

        this.Ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.Ship.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.Ship.position = this.transform.FindChild("StartingBlock").position;
        this.Ship.rotation = this.transform.FindChild("StartingBlock").rotation;
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
            Reset();
    }

    private void FixedUpdate()
    {
        if ((this.currentLapRecord != null) && !this.currentLapRecord.IsComplete)
            this.elapsedTime += Time.fixedDeltaTime;
    }

    private void OnCheckpointTriggered(Checkpoint cp)
    {
        if ((cp.Id == this.checkpoints.First().Id) && (this.currentLapRecord == null))
            this.currentLapRecord = new LapRecord();

        if (this.currentLapRecord != null)
            this.currentLapRecord.AddIntermediate(new LapIntermediateRecord(cp.Id, this.elapsedTime));
    }
}
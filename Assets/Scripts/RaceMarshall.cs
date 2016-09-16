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

    public bool TryAddIntermediate(LapIntermediateRecord record)
    {
        if (!IsComplete)
        {
            if (this.intermediateRecords.Any())
            {
                if ((record.CheckpointId == this.intermediateRecords.Last().CheckpointId + 1) || (record.CheckpointId == this.intermediateRecords.First().CheckpointId))
                {
                    this.intermediateRecords.Add(record);
                    return true;
                }
            }
            else
            {
                this.intermediateRecords.Add(record);
                return true;
            }
        }

        return false;
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
    private AudioSource checkpointSample;
    private AudioSource bestLapSample;

    private float elapsedTime;
    private LapRecord currentLapRecord;
    private LapRecord bestLapRecord;

    public Transform Ship;

    public float ElapsedTime { get { return this.elapsedTime; } }
    public LapRecord CurrentLapRecord { get { return this.currentLapRecord; } }
    public LapRecord BestLapRecord { get { return this.bestLapRecord; } }

    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().OrderBy(cp => cp.Id).ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);

        this.checkpointSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "CheckpointSample"));
        this.bestLapSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "BestLapSample"));

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
        if (Input.GetButtonDown("Restart"))
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
        {
            if (this.currentLapRecord.TryAddIntermediate(new LapIntermediateRecord(cp.Id, this.elapsedTime)))
                this.checkpointSample.Play();

            if (this.currentLapRecord.IsComplete)
            {
                if ((this.bestLapRecord == null) || (this.bestLapRecord.Time > this.currentLapRecord.Time))
                {
                    this.bestLapRecord = this.currentLapRecord;
                    this.bestLapSample.Play();
                }
            }
        }
    }
}
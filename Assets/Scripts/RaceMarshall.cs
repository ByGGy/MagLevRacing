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
    private readonly List<LapIntermediateRecord> intermediates;

    public LapRecord()
    {
        this.Id = ++Qty;
        this.intermediates = new List<LapIntermediateRecord>();
    }

    public bool IsComplete
    {
        get { return (this.intermediates.Count() > 1) && (this.intermediates.First().CheckpointId == this.intermediates.Last().CheckpointId); }
    }

    public float Time
    {
        get { return this.intermediates.Any() ? this.intermediates.Last().ElapsedTime : 0; }
    }

    public IEnumerable<LapIntermediateRecord> Intermediates { get { return this.intermediates; } }

    public bool TryAddIntermediate(LapIntermediateRecord record)
    {
        if (!IsComplete)
        {
            if (this.intermediates.Any())
            {
                if ((record.CheckpointId == this.intermediates.Last().CheckpointId + 1) || (record.CheckpointId == this.intermediates.First().CheckpointId))
                {
                    this.intermediates.Add(record);
                    return true;
                }
            }
            else
            {
                this.intermediates.Add(record);
                return true;
            }
        }

        return false;
    }
}

public class TimeAttackAttempt
{
    private readonly LapRecord referenceLap;
    private readonly LapRecord currentLap;
    private float elapsedTime;

    public TimeAttackAttempt(LapRecord referenceLap)
    {
        this.referenceLap = referenceLap;
        this.currentLap = new LapRecord();
        this.elapsedTime = 0;
    }

    public bool IsComplete { get { return this.currentLap.IsComplete; } }

    public float ElapsedTime { get { return this.elapsedTime; } }
    public float Time { get { return this.currentLap.Time; } }

    public LapRecord CurrentLap { get { return this.currentLap; } }

    public IEnumerable<float> Gaps
    {
        get
        {
            List<float> values = new List<float>();
            for (int i = 0; i < this.currentLap.Intermediates.Count(); i++)
            {
                values.Add(this.currentLap.Intermediates.ElementAt(i).ElapsedTime - this.referenceLap.Intermediates.ElementAt(i).ElapsedTime);
            }
            return values;
        }
    }

    public void UpdateClock(float deltaTime)
    {
        this.elapsedTime += deltaTime;
    }

    public bool TryAddIntermediate(Checkpoint cp)
    {
        return this.currentLap.TryAddIntermediate(new LapIntermediateRecord(cp.Id, this.elapsedTime));
    }
}

public class RaceMarshall : MonoBehaviour
{
    private List<Checkpoint> checkpoints;
    private AudioSource checkpointSample;
    private AudioSource bestLapSample;

    private TimeAttackAttempt currentAttempt;
    private LapRecord bestLap;

    public Transform Ship;

    public TimeAttackAttempt CurrentAttempt { get { return this.currentAttempt; } }
    public LapRecord BestLap { get { return this.bestLap; } }

    private void Start()
	{
	    this.checkpoints = GetComponentsInChildren<Checkpoint>().OrderBy(cp => cp.Id).ToList();
        this.checkpoints.ForEach(cp => cp.OnTargetDetected += OnCheckpointTriggered);

        this.checkpointSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "CheckpointSample"));
        this.bestLapSample = GetComponentsInChildren<AudioSource>().FirstOrDefault(source => string.Equals(source.name, "BestLapSample"));

        Restart();
	}

    private void Restart()
    {
        this.currentAttempt = null;

        this.Ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.Ship.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.Ship.position = this.transform.FindChild("StartingBlock").position;
        this.Ship.rotation = this.transform.FindChild("StartingBlock").rotation;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
            Restart();
    }

    private void FixedUpdate()
    {
        if ((this.currentAttempt != null) && !this.currentAttempt.IsComplete)
            this.currentAttempt.UpdateClock(Time.fixedDeltaTime);
    }

    private void OnCheckpointTriggered(Checkpoint cp)
    {
        if ((cp.Id == this.checkpoints.First().Id) && (this.currentAttempt == null))
            this.currentAttempt = new TimeAttackAttempt(this.bestLap);

        if (this.currentAttempt != null)
        {
            if (this.currentAttempt.TryAddIntermediate(cp))
                this.checkpointSample.Play();

            if (this.currentAttempt.IsComplete)
            {
                if ((this.bestLap == null) || (this.bestLap.Time > this.currentAttempt.Time))
                {
                    this.bestLap = this.currentAttempt.CurrentLap;
                    this.bestLapSample.Play();
                }
            }
        }
    }
}
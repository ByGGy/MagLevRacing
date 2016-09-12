using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LapUI : MonoBehaviour
{
    public Transform Source;

    private Text bestLapText;
    private Text currentLapText;
    private Text gapText;

    private void Start()
    {
        this.bestLapText = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "BestLapText")).FirstOrDefault();
        this.currentLapText = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "CurrentLapText")).FirstOrDefault();
        this.gapText = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "GapText")).FirstOrDefault();
    }

    private string FormatTime(float timeInSeconds)
    {
        bool isNegative = timeInSeconds < 0;
        TimeSpan span = TimeSpan.FromSeconds(Math.Abs(timeInSeconds));
        return string.Format("{0}{1:00}'{2:00}\"{3:000}", isNegative ? "-" : string.Empty, span.Minutes, span.Seconds, span.Milliseconds);
    }

    private string FormatGap(float timeInSeconds)
    {
        bool isNegative = timeInSeconds < 0;
        return string.Format("{0} {1}", isNegative ? "-" : "+", FormatTime(Math.Abs(timeInSeconds)));
    }

	// Update is called once per frame
	private void Update()
	{
        RaceMarshall marshall = this.Source.GetComponent<RaceMarshall>();
	    if (marshall.BestLapRecord != null)
            this.bestLapText.text = string.Format("Best Lap : #{0} - {1}", marshall.BestLapRecord.Id + 1, FormatTime(marshall.BestLapRecord.Time));

        if (marshall.CurrentLapRecord != null)
        {
            StringBuilder builder = new StringBuilder();
	        builder.AppendLine(string.Format("Lap #{0} : {1}", marshall.CurrentLapRecord.Id +1, FormatTime(marshall.ElapsedTime)));
            marshall.CurrentLapRecord.IntermediateRecords.ToList().ForEach(r => builder.AppendLine(string.Format("   |- Checkpoint #{0} : {1}", r.CheckpointId, FormatTime(r.ElapsedTime))));
            this.currentLapText.text = builder.ToString();
        }

	    if (marshall.BestLapRecord != null && marshall.CurrentLapRecord != null)
	    {
            StringBuilder builder = new StringBuilder();
	        builder.AppendLine();
	        for (int i = 0; i < marshall.CurrentLapRecord.IntermediateRecords.Count(); i++)
	        {
                builder.AppendLine(FormatGap(marshall.CurrentLapRecord.IntermediateRecords.ElementAt(i).ElapsedTime - marshall.BestLapRecord.IntermediateRecords.ElementAt(i).ElapsedTime));
	        }

            this.gapText.text = builder.ToString();	        
	    }
	}
}

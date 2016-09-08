using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LapUI : MonoBehaviour
{
    public Transform Source;

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan span = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:00}'{1:00}\"{2:000}", span.Minutes, span.Seconds, span.Milliseconds);
    }

	// Update is called once per frame
	private void Update()
	{
        StringBuilder builder = new StringBuilder();

        RaceMarshall marshall = this.Source.GetComponent<RaceMarshall>();
	    if (marshall.BestLapRecord != null)
	    {
            builder.AppendLine(string.Format("Best Lap : #{0} - {1}", marshall.BestLapRecord.Id + 1, FormatTime(marshall.BestLapRecord.Time)));
	    }

        if (marshall.CurrentLapRecord != null)
        {
	        builder.AppendLine(string.Format("Lap #{0} : {1}", marshall.CurrentLapRecord.Id +1, FormatTime(marshall.ElapsedTime)));
            marshall.CurrentLapRecord.IntermediateRecords.ToList().ForEach(r => builder.AppendLine(string.Format("   |- Checkpoint #{0} : {1}", r.CheckpointId, FormatTime(r.ElapsedTime))));
        }

        GetComponent<Text>().text = builder.ToString();
	}
}

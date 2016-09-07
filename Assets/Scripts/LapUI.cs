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
        RaceMarshall marshall = this.Source.GetComponent<RaceMarshall>();
        
        StringBuilder builder = new StringBuilder();
	    builder.AppendLine(string.Format("Lap Time : {0}", FormatTime(marshall.ElapsedTime)));
        marshall.IntermediateRecords.ToList().ForEach(r => builder.AppendLine(string.Format("   Checkpoint {0} : {1}", r.CheckpointId, FormatTime(r.ElapsedTime))));

        GetComponent<Text>().text = builder.ToString();
	}
}

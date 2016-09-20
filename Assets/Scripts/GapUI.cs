using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GapUI : MonoBehaviour
{
    public Transform Source;

    private Text title;
    private Text content;

    private void Start()
    {
        this.title = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Title")).FirstOrDefault();
        this.title.text = "Gaps";

        this.content = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Content")).FirstOrDefault();
        this.content.text = string.Empty;
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
        return string.Format("<color={0}>{1} {2}</color>", isNegative ? "#00ff00ff" : "#ff0000ff", isNegative ? "-" : "+", FormatTime(Math.Abs(timeInSeconds)));
    }

    private void Update()
    {
        RaceMarshall marshall = this.Source.GetComponent<RaceMarshall>();
        if (marshall.CurrentAttempt != null)
        {
            StringBuilder builder = new StringBuilder();
            marshall.CurrentAttempt.Gaps.Skip(1).Select(g => FormatGap(g)).ToList().ForEach(info => builder.AppendLine(info));
            this.content.text = builder.ToString();
        }
    }
}

﻿using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CurrentLapUI : MonoBehaviour
{
    public Transform Source;

    private Text title;
    private Text content;

    private void Start()
    {
        this.title = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Title")).FirstOrDefault();
        this.title.text = "Current lap";

        this.content = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Content")).FirstOrDefault();
        this.content.text = string.Empty;
    }

    private string FormatTime(float timeInSeconds)
    {
        bool isNegative = timeInSeconds < 0;
        TimeSpan span = TimeSpan.FromSeconds(Math.Abs(timeInSeconds));
        return string.Format("{0}{1:00}'{2:00}\"{3:000}", isNegative ? "-" : string.Empty, span.Minutes, span.Seconds, span.Milliseconds);
    }

    private void Update()
    {
        RaceMarshall marshall = this.Source.GetComponent<RaceMarshall>();
        if (marshall.CurrentAttempt != null)
        {
            this.title.text = string.Format("Current lap #{0}. {1}", marshall.CurrentAttempt.CurrentLap.Id + 1, FormatTime(marshall.CurrentAttempt.ElapsedTime));

            StringBuilder builder = new StringBuilder();
            marshall.CurrentAttempt.CurrentLap.Intermediates.Skip(1).Select((r , i) => string.Format("{0}. {1}", i+1, FormatTime(r.ElapsedTime))).ToList().ForEach(info => builder.AppendLine(info));
            this.content.text = builder.ToString();
        }
    }
}
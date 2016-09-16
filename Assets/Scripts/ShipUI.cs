using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    public Transform Source;

    private Text title;
    private Text content;

    private void Start()
    {
        this.title = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Title")).FirstOrDefault();
        this.title.text = "Speed";

        this.content = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "Content")).FirstOrDefault();
        this.content.text = string.Empty;
    }

    private void Update()
	{
        int velocityInKmPerHour = (int)(this.Source.GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000.0f);
        this.content.text = string.Format("{0}Km/h", velocityInKmPerHour);
	}
}

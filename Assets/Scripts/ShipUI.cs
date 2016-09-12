using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    public Transform Source;

    private Text shipText;

    private void Start()
    {
        this.shipText = GetComponentsInChildren<Text>().Where(c => string.Equals(c.name, "ShipText")).FirstOrDefault();
    }

	// Update is called once per frame
    private void Update()
	{
        int velocityInKmPerHour = (int)(this.Source.GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000.0f);
        this.shipText.text = string.Format("Speed : {0}Km/h", velocityInKmPerHour);
	}
}

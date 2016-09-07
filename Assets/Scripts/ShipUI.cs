using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    public Transform Source;

	// Update is called once per frame
    private void Update()
	{
        int velocityInKmPerHour = (int)(this.Source.GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000.0f);
	    GetComponent<Text>().text = string.Format("Speed : {0}Km/h", velocityInKmPerHour);
	}
}

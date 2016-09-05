using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EasyUI : MonoBehaviour
{
    public Transform Ship;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        int velocityInKmPerHour = (int)(this.Ship.GetComponent<Rigidbody>().velocity.magnitude * 3600 / 1000.0f);
	    GetComponent<Text>().text = string.Format("{0} Km/h", velocityInKmPerHour);
	}
}

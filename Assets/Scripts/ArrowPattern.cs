using UnityEngine;

public class ArrowPattern : MonoBehaviour
{
    private const float MAX = 3;

    private int sign;
    private float factor;

    public float Frequency = 1;
    public float Delay = 0;

	// Use this for initialization
	void Start ()
	{
	    this.sign = 1;
        this.factor = -this.Delay;
	}
	
	// Update is called once per frame
	void Update ()
	{
        this.factor = this.factor + Time.deltaTime * this.Frequency * this.sign;
	    if (this.factor < 0)
	        this.sign = 1;
        if (this.factor > MAX)
	        this.sign = -1;

	    Material material = GetComponent<Renderer>().material;

	    Color finalColor = material.GetColor("_Color") * this.factor;
	    GetComponent<Renderer>().material.SetColor("_EmissionColor", finalColor);
	    //DynamicGI.SetEmissive(GetComponent<Renderer>(), new Color(1f, 0.0f, 0.0f, 1.0f) * Random.value);
	}
}

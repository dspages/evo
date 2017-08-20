using UnityEngine;
using System.Collections;

public class FoodOsc : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 ((Random.value-0.5f)*15,0.0f,(Random.value-0.5f)*15);
	}
	public int Intensity;
	// Update is called once per frame

	void Update () {		
		if (Intensity == 0)
			Intensity = 2;
		else if (Intensity == 2)
			Intensity = 4;
		else if (Intensity == 4)
			Intensity = 8;
		else if (Intensity == 8)
			Intensity = 16;
        else if (Intensity == 16)
            Intensity = 32;
        else if (Intensity == 32)
            Intensity = 0;
    }
}

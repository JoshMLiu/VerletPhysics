using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindText : MonoBehaviour {

	GameObject wind;
	Properties props;

	// Use this for initialization
	void Start () {
		wind = GameObject.Find ("GameInitializer");
		props = (Properties) wind.GetComponent ("Properties");
	}

	// Update is called once per frame
	void Update () {
		// Get and display wind from properties
		float windspeed = props.GetWindforce ();
		float newspeed = 100000f * windspeed;
		TextMesh windtext = (TextMesh) transform.GetComponent<TextMesh> ();
		string m1 = "Windspeed: ";
		string m2 = newspeed.ToString ();
		string message = System.String.Concat(m1, m2);
		windtext.text = message;
	}
}
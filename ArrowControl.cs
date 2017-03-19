using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour {

	// displays the wind with an arrow
	GameObject wind;
	Properties props;

	void Start () {
		wind = GameObject.Find ("GameInitializer");
		props = (Properties) wind.transform.GetComponent ("Properties");
	}
	
	void Update () {
		float windforce = props.GetWindforce ();
		float maxforce = props.GetWindMax ();
		// left or right and scale the length
		transform.rotation = Quaternion.Euler (0, 0, -90f);
		transform.localScale = new Vector3 (1, windforce/maxforce, 1);
	}
}

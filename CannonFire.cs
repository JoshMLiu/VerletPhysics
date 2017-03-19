using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFire : MonoBehaviour {

	Properties props; 
	// cannon angle
	float minangle = 5f;
	float maxangle = 81f;

	void Start () {
		GameObject gi = GameObject.Find ("GameInitializer");
		props = (Properties) gi.transform.GetComponent ("Properties");
	}
	
	void Update () {

		// Light cannon if it is selected
		Transform canbase = transform.Find("base");
		Behaviour halo = (Behaviour) canbase.GetComponent("Halo");

		if (props.IsGoatCannon ()) {
			halo.enabled = false;
		} else {
			halo.enabled = true;
		}

		// Cannon fired 
		if (Input.GetKeyDown (KeyCode.Space) && !props.IsGoatCannon()) {

			// set random angle and create an object with a cannonball with velocity at that angle
			float randangle = Random.Range (minangle, maxangle);
			Vector3 tempvelocity = new Vector3 (0.23f, 0, 0);
			Quaternion q = Quaternion.Euler (0, 0, randangle);
			Quaternion q2 = Quaternion.Euler (-randangle, 90f, 0);
			tempvelocity = q * tempvelocity;

			GameObject control = GameObject.CreatePrimitive(PrimitiveType.Cube);
			control.transform.position = new Vector3 (-20, -20, 0);

			Cannonball cb = control.AddComponent<Cannonball> ();
			Transform bar = transform.Find ("barrel");
			bar.rotation = q2;
			cb.pos = transform.position + new Vector3(-0.2f, 0.2f, 0.3f);
			cb.velocity = tempvelocity;

		}
	}

}

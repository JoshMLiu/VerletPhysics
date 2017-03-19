using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatCannonFire : MonoBehaviour {

	Properties props; 
	float minangle = 15f;
	float maxangle = 81f;
	public float currentangle = 0f;

	// Use this for initialization
	void Start () {
		GameObject gi = GameObject.Find ("GameInitializer");
		props = (Properties) gi.transform.GetComponent ("Properties");
	}

	// Update is called once per frame
	void Update () {

		// Light halo if selected
		Transform canbase = transform.Find("base");
		Behaviour halo = (Behaviour) canbase.GetComponent("Halo");

		if (props.IsGoatCannon ()) {
			halo.enabled = true;
		} else {
			halo.enabled = false;
		}

		if (Input.GetKeyDown (KeyCode.Space) && props.IsGoatCannon()) {

			float randangle = Random.Range (minangle, maxangle);
			currentangle = randangle;
			Quaternion q2 = Quaternion.Euler (-randangle, -90f, 0);

			GameObject spawner = GameObject.Find ("GoatSpawner");
			// If there are more than 21 goats, don't spawn any more because it lags the game
			if (spawner.transform.childCount < 22) {
				GameObject prefab = Instantiate (Resources.Load ("GoatSpawn")) as GameObject;
				prefab.transform.parent = spawner.transform;
			}
				
			Transform bar = transform.Find ("barrel");
			bar.rotation = q2;

		}
	}


}

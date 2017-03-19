using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties : MonoBehaviour {

	// class for keeping all the often used properties

	private readonly float gravity = -0.0031f;
	private readonly float windmin = -0.001f;
	private readonly float windmax = 0.001f;
	private readonly float airresistance = 0.00005f;

	private float windforce;
	// says which cannon to fire
	private bool firegoat;

	void Start() {
		windforce = 0;
		// change wind directio nevery 0.5s
		InvokeRepeating("ChangeWind", 1.0f, 0.5f);
		firegoat = false;
	}

	public float GetGravity() {
		return gravity;
	}

	public float GetWindforce() {
		return windforce;
	}

	public float GetWindMax() {
		return windmax;
	}
	 
	public bool IsGoatCannon() {
		return firegoat;
	}

	public float GetAirRes() {
		return airresistance;
	}

	public void ChangeCannon() {
		if (firegoat) {
			firegoat = false;
		} else {
			firegoat = true;
		}
	}

	void ChangeWind() {
		windforce = Random.Range (windmin, windmax);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			ChangeCannon();
		}
	}

}

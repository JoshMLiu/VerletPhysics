using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHit : MonoBehaviour {

	// for cannonball exiting bounds
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Delete") {
			GameObject p = transform.parent.gameObject;
			Destroy (p);
		}
	}
}

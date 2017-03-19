using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified from the tutorial at https://gamedevelopment.tutsplus.com/tutorials/simulate-tearable-cloth-and-ragdolls-with-simple-verlet-integration--gamedev-519
public class PointLink {

	// distance that the link naturally goes to
	float restingdistance;
	// how much the link can stretch
	float stiffness;

	// To render the line
	GameObject linkline;
	LineRenderer line;

	PointMass point1;
	PointMass point2;

	private bool isVisible;

	public PointLink(PointMass a, PointMass b, float r, float s, bool v, Color c) {

		point1 = a;
		point2 = b;
		restingdistance = r;
		stiffness = s;
		isVisible = v;

		Vector3[] points = new Vector3[2];
		points [0] = new Vector3 (point1.position.x, point1.position.y, 0);
		points [1] = new Vector3 (point2.position.x, point2.position.y, 0);

		GameObject goatlist = GameObject.Find ("GoatLines");

		// initialize renderer
		if (isVisible) {
			linkline = new GameObject ();
			line = linkline.AddComponent<LineRenderer> ();
			Material color;
			if (c == Color.white) {
				color = Resources.Load ("white", typeof(Material)) as Material;
			} else if (c == Color.gray) {
				color = Resources.Load ("grey", typeof(Material)) as Material;
			} else if (c == Color.yellow) {
				color = Resources.Load ("yellow", typeof(Material)) as Material;
			}
			else {
				color = Resources.Load ("black", typeof(Material)) as Material;
			}
			line.material = color;
			line.startWidth = 0.03f;
			line.endWidth = 0.03f;
			linkline.transform.parent = goatlist.transform;
			line.SetPositions (points);
		}

	}

	public void Solve() {

		// Pulls the two points together or pushes them apart 
		Vector2 diffvector = point1.position - point2.position;
		float distance = diffvector.magnitude;
		float difference = (restingdistance - distance) / distance;

		float imt1 = 1 / point1.mass;
		float imt2 = 1 / point2.mass;
		float scalar1 = (imt1 / (imt1 + imt2)) * stiffness;
		float scalar2 = stiffness - scalar1;

		Vector2 translate1 = Vector2.Scale (new Vector2 (scalar1 * difference, scalar1 * difference), diffvector);
		Vector2 translate2 = Vector2.Scale (new Vector2 (scalar2 * difference, scalar2 * difference), diffvector);

		point1.position = point1.position + translate1;
		point2.position = point2.position - translate2;
	
	}

	public void Render() {
		if (isVisible) {
			Vector3[] points = new Vector3[2];
			points [0] = new Vector3 (point1.position.x, point1.position.y, 0);
			points [1] = new Vector3 (point2.position.x, point2.position.y, 0);
			line.SetPositions (points);
		}
	}

	public void Destroy() {
		GameObject.Destroy (linkline);
	}
		
}

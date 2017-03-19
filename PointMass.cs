using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified from the tutorial at https://gamedevelopment.tutsplus.com/tutorials/simulate-tearable-cloth-and-ragdolls-with-simple-verlet-integration--gamedev-519
public class PointMass {

	// Verlet integration
	public Vector2 prevposition;
	public Vector2 position;
	public Vector2 acceleration;
	public float mass;

	// Anchor keeps the point in one location until it is unhinged
	bool isAnchor;
	bool anchored;
	Vector2 anchor;
	bool isVisible;

	GameObject gi;
	public GameObject pointrender;

	public ArrayList links = new ArrayList ();

	// Give point initial velocity
	public PointMass(Vector2 initialpos, Vector2 initialvelocity, float m,  bool anc, bool v) {

		position = initialpos;
		prevposition = initialpos - initialvelocity;
		acceleration = new Vector2 ();
		mass = m;
		isAnchor = anc;
		anchored = false;
		anchor = new Vector2 ();
		gi = GameObject.Find ("GameInitializer");
		isVisible = v;

		if (isVisible) {
			// put point in goatlines
			GameObject goatlist = GameObject.Find ("GoatLines");
			pointrender = GameObject.Instantiate(Resources.Load ("Point", typeof(GameObject)) as GameObject);
			pointrender.transform.parent = goatlist.transform;
			pointrender.transform.position = new Vector3 (position.x, position.y, 0);
		}

	}

	// No velocity 
	public PointMass(Vector2 initialpos, float m,  bool anc, bool v) {

		position = initialpos;
		prevposition = initialpos;
		acceleration = new Vector2 ();
		mass = m;
		isAnchor = anc;
		anchored = false;
		anchor = new Vector2 ();
		gi = GameObject.Find ("GameInitializer");
		isVisible = v;

		if (isVisible) {
			GameObject goatlist = GameObject.Find ("GoatLines");
			pointrender = GameObject.Instantiate(Resources.Load ("Point", typeof(GameObject)) as GameObject);
			pointrender.transform.parent = goatlist.transform;
			pointrender.transform.position = new Vector3 (position.x, position.y, 0);
		}
	
	}
		
	public void UpdatePhysics() {
		SetAcceleration ();
		Vector2 newpos = NextPosition ();
		ChangePosition (newpos);
	}

	public void Render() {
		// render all links coming from this point
		for (int i = 0; i < links.Count; i++) {
			PointLink currlink = (PointLink) links [i];
			currlink.Render (); 
		}
		if (isVisible) {
			pointrender.transform.position = new Vector3 (position.x, position.y, 0);
		}
	}

	// called multiple times per frame
	public bool SolveConstraints() {
		// Stretch or pull all points from links
		for (int i = 0; i < links.Count; i++) {
			PointLink currlink = (PointLink) links [i];
			currlink.Solve ();
		}

		// Skip most if point is anchored (cannot change)
		if (!anchored) {

			// Boundary Constraints, true return deletes the goat
			if (position.y < -4f) {
				return true;
			}
			if (position.y > 11f) {
				return true;
			}
			if (position.x > 11f) {
				return true;
			}
			if (position.x < -11f) {
				return true;
			}

			// Constraints to hit mountain
			CreateMountain mountain = (CreateMountain)gi.GetComponent ("CreateMountain");
			TriangleStruct[] left = mountain.lefttriangles;
			TriangleStruct[] right = mountain.righttriangles;
			TriangleStruct top = mountain.toptriangle;

			if (position.y <= 2.0f) {
				// left
				if (position.x >= -5.0f && position.x <= -1.0f) {
					for (int i = 0; i < left.Length; i++) {
						TriangleStruct lefttri = (TriangleStruct)left [i];
						if (lefttri.PointInTriangle (position)) {
							position = lefttri.Relocate (position);
							if (isAnchor) {
								anchored = true;
								anchor = position;
							}
						}
					}
				}
				// right
				else if (position.x >= 1.0f && position.x <= 5.0f) {
					for (int i = 0; i < right.Length; i++) {
						TriangleStruct righttri = (TriangleStruct)right [i];
						if (righttri.PointInTriangle (position)) {
							position = righttri.Relocate (position);
							if (isAnchor) {
								anchored = true;
								anchor = position;
							}
						}
					}
				}
				// middle
				else if (position.x >= -1.0f && position.x <= 1.0f) {
					if (top.PointInTriangle (position)) {
						position = top.Relocate (position);
						if (isAnchor) {
							anchored = true;
							anchor = position;
						}
					}
				}

			}

		}
		// pin to mountain if anchored by a foot
		if (anchored) {
			position = anchor;
		}
		return false;
	}

	// creates a link between this point and another
	public void AttachTo(PointMass p, float restingdist, float stiff, bool visible, Color c) {
		PointLink link = new PointLink (this, p, restingdist, stiff, visible, c);
		links.Add (link);
	}

	public void RemoveLink(PointLink l) {
		links.Remove (l);
	}

	// uses game properties
	void SetAcceleration() {
		Properties props = (Properties)gi.GetComponent ("Properties");
		float gravity = props.GetGravity ();
		float windspeed = props.GetWindforce ();
		float airres = props.GetAirRes ();
		acceleration = new Vector2(windspeed + airres, gravity);
	}

	// Verlete velocity
	Vector2 GetVelocity() {
		float velX = position.x - prevposition.x;
		float velY = position.y - prevposition.y;
		return new Vector2 (velX, velY);
	}

	Vector2 NextPosition() {
		Vector2 velocity = GetVelocity ();
		return position + velocity + acceleration;
	}

	// Go to a new position
	void ChangePosition(Vector2 newpos) {
		prevposition = position;
		position = newpos;
	}

	public void AnchorTo(Vector2 location) {
		if (isAnchor) {
			anchored = true;
			anchor = location;
		}
	}

	// Gives this point more velocity
	public void GiveVelocity(Vector2 velocity) {
		position = position + velocity;
	}

	public void Unhinge() {
		isAnchor = false;
		anchored = false;
	}

}

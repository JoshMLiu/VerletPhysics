using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatPoints : MonoBehaviour {

	public ArrayList points = new ArrayList();

	// Number of constraint solves to do
	int numPhysicsUpdates = 3;
	Vector2 trans;
	GoatCannonFire cannon;

	// For bounding box
	public PointMass center;
	public float radius;

	void Start () {

		// Get cannon and get velocity 
		GameObject c = GameObject.Find ("Goatcannon");
		cannon = (GoatCannonFire) c.GetComponent ("GoatCannonFire");

		Vector3 tempvelocity = new Vector3 (1.36f, 0, 0);
		Quaternion q = Quaternion.Euler (0, 0, 180-cannon.currentangle);
		tempvelocity = q * tempvelocity;

		GameObject spawner = GameObject.Find ("GoatSpawner");
		Vector3 temptrans = spawner.transform.position;
		// translate to the spawner location (goat points are defined in local coordinates)
		trans = new Vector2 (temptrans.x, temptrans.y);
		radius = 1f;

		float scale = 0.7f;

		// GOAT POINTS in local coordinates, velocity given to nose chin and forehead

		// head + neck                             
		PointMass nose = new PointMass(Shift(new Vector2 (-0.6f*scale, 0.07f*scale)), tempvelocity, 1, false, false);
		PointMass chin = new PointMass(Shift(new Vector2 (-0.6f*scale, -0.03f*scale)), tempvelocity, 1, false, false);
		PointMass forehead = new PointMass(Shift(new Vector2 (-0.35f*scale, 0.32f*scale)), tempvelocity, 1, false, false);
		PointMass horntip = new PointMass(Shift(new Vector2 (-0.1f*scale, 0.5f*scale)), 1, false, false);
		PointMass headback = new PointMass(Shift(new Vector2 (-0.26f*scale, 0.3f*scale)), 1, false, false);
		PointMass neckback = new PointMass(Shift(new Vector2 (-0.1f*scale, 0.22f*scale)), 1, false, false);
		PointMass beardfront = new PointMass(Shift(new Vector2 (-0.48f*scale, 0f*scale)), 1, false, false);
		PointMass beardback = new PointMass(Shift(new Vector2 (-0.36f*scale, 0.04f*scale)), 1, false, false);
		PointMass beardtip = new PointMass(Shift(new Vector2 (-0.4f*scale, -0.2f*scale)), 1, false, false);
		PointMass neckfront = new PointMass(Shift(new Vector2 (-0.24f*scale, 0.06f*scale)), 1, false, false);
		PointMass eye = new PointMass(Shift(new Vector2 (-0.35f*scale, 0.2f*scale)), 1, false, true);

		// body
		PointMass butt = new PointMass(Shift(new Vector2 (0.6f*scale, 0.2f*scale)), 1, false, false);
		PointMass fronthip = new PointMass(Shift(new Vector2 (0.1f*scale, -0.14f*scale)), 1, false, false);
		PointMass backhip = new PointMass(Shift(new Vector2 (0.52f*scale, -0.2f*scale)), 1, false, false);
		center = fronthip;

		// legs
		PointMass frontknee = new PointMass(Shift(new Vector2 (0.1f*scale, -0.36f*scale)), 1, false, false);
		PointMass frontfoot = new PointMass(Shift(new Vector2 (0.1f*scale, -0.46f*scale)), 1, true, false);
		PointMass backknee = new PointMass(Shift(new Vector2 (0.52f*scale, -0.4f*scale)), 1, false, false);
		PointMass backfoot = new PointMass(Shift(new Vector2 (0.52f*scale, -0.5f*scale)), 1, true, false);

		// Visible links
		// head
		nose.AttachTo (forehead, Dist (nose, forehead), 0.6f, true, Color.white);
		nose.AttachTo (chin, Dist (nose, chin), 0.6f, true, Color.white);
		forehead.AttachTo (horntip, Dist (forehead, horntip), 0.6f, true, Color.black);
		forehead.AttachTo (headback, Dist (forehead, headback), 0.6f, true, Color.white);
		headback.AttachTo (horntip, Dist (headback, horntip), 0.6f, true, Color.black);
		headback.AttachTo (neckback, Dist (headback, neckback), 0.6f, true, Color.white);
		chin.AttachTo (beardfront, Dist (chin, beardfront), 0.6f, true, Color.white);
		beardfront.AttachTo (beardback, Dist (beardfront, beardback), 0.6f, true, Color.white);
		beardfront.AttachTo (beardtip, Dist (beardfront, beardtip), 0.6f, true, Color.gray);
		beardback.AttachTo (beardtip, Dist (beardback, beardtip), 0.6f, true, Color.gray);
		beardback.AttachTo (neckfront, Dist (beardback, neckfront), 0.6f, true, Color.white);
		// body
		neckfront.AttachTo (fronthip, Dist (neckfront, fronthip), 0.6f, true, Color.white);
		fronthip.AttachTo (backhip, Dist (fronthip, backhip), 0.6f, true, Color.white);
		neckback.AttachTo (butt, Dist (neckback, butt), 0.6f, true, Color.white);
		butt.AttachTo (backhip, Dist (butt, backhip), 0.6f, true, Color.white);
		// legs
		fronthip.AttachTo (frontknee, Dist (fronthip, frontknee), 0.6f, true, Color.black);
		frontknee.AttachTo (frontfoot, Dist (frontknee, frontfoot), 0.6f, true, Color.black);
		backhip.AttachTo (backknee, Dist (backhip, backknee), 0.6f, true, Color.black);
		backknee.AttachTo (backfoot, Dist (backknee, backfoot), 0.6f, true, Color.black);

		// Invisible links
		// head
		forehead.AttachTo (eye, Dist (forehead, eye), 0.6f, false, Color.black);
		nose.AttachTo (eye, Dist (nose, eye), 0.6f, false, Color.black);
		neckfront.AttachTo (eye, Dist (neckfront, eye), 0.6f, false, Color.black);
		nose.AttachTo (butt, Dist (nose, butt), 0.6f, false, Color.black);
		nose.AttachTo (fronthip, Dist (nose, fronthip), 0.6f, false, Color.black);
		chin.AttachTo (beardtip, Dist (chin, beardtip), 0.6f, false, Color.black);
		chin.AttachTo (butt, Dist (chin, butt), 0.6f, false, Color.black);
		chin.AttachTo (horntip, Dist (chin, horntip), 0.6f, false, Color.black);
		chin.AttachTo (forehead, Dist (chin, forehead), 1f, false, Color.black);
		forehead.AttachTo (beardtip, Dist (forehead, beardtip), 0.6f, false, Color.black);
		neckfront.AttachTo (neckback, Dist (neckfront, neckback), 0.6f, false, Color.black);
		neckfront.AttachTo (forehead, Dist (neckfront, forehead), 1f, false, Color.black);
		beardtip.AttachTo (horntip, Dist (beardtip, horntip), 0.6f, false, Color.black);
		beardtip.AttachTo (nose, Dist (beardtip, nose), 0.6f, false, Color.black);
		beardtip.AttachTo (fronthip, Dist (beardtip, fronthip), 0.6f, false, Color.black);
		// body
		butt.AttachTo (horntip, Dist (butt, horntip), 0.6f, false, Color.black);
		fronthip.AttachTo (butt, Dist (fronthip, butt), 0.6f, false, Color.black);
		fronthip.AttachTo (horntip, Dist (fronthip, horntip), 0.6f, false, Color.black);
		backhip.AttachTo (forehead, Dist (backhip, forehead), 0.6f, false, Color.black);
		backhip.AttachTo (horntip, Dist (backhip, horntip), 0.6f, false, Color.black);
		// legs
		frontfoot.AttachTo (beardtip, Dist (frontfoot, beardtip), 0.1f, false, Color.black);
		frontfoot.AttachTo (horntip, Dist (frontfoot, horntip), 0.6f, false, Color.black);
		frontfoot.AttachTo (butt, Dist (frontfoot, butt), 0.1f, false, Color.black);
		backfoot.AttachTo (fronthip, Dist (backfoot, fronthip), 0.1f, false, Color.black);
		backfoot.AttachTo (butt, Dist (backfoot, butt), 0.1f, false, Color.black);
		backfoot.AttachTo (horntip, Dist (backfoot, horntip), 0.6f, false, Color.black);

		// Add points to goatpoint list
		points.Add(nose);
		points.Add(chin);
		points.Add(forehead); 
		points.Add(horntip); 
		points.Add(headback);
		points.Add(neckback);
		points.Add(beardfront);
		points.Add(beardback);
		points.Add(beardtip);
		points.Add(neckfront);
		points.Add(eye);
		points.Add(butt); 
		points.Add(fronthip); 
		points.Add(backhip); 
		points.Add(frontknee); 
		points.Add(frontfoot); 
		points.Add(backknee); 
		points.Add(backfoot); 

	}

	// distance from one point to another
	float Dist(PointMass p1, PointMass p2) {
		return (p1.position - p2.position).magnitude;
	}

	// adds two vectors
	Vector2 Shift(Vector2 v) {
		return v + trans;
	}
	
	// Update is called once per frame
	void Update () {

		// Update positions
		for (int i = 0; i < points.Count; i++) {
			PointMass p = (PointMass) points [i];
			p.UpdatePhysics();
		}
			
		// just loop some number of times
		for (int i = 0; i < numPhysicsUpdates; i++) {
			// go through each point and solve the constraints while moving other points
			for (int k = 0; k < points.Count; k++) {
				PointMass pc = (PointMass) points [k];
				// returns true if goat hits the floor or a wall
				bool delete = pc.SolveConstraints ();
				if (delete) {
					Delete ();
				}
			}
		}
			
		// render
		for (int i = 0; i < points.Count; i++) {
			PointMass m = (PointMass) points [i];
			// Recursively renders lines
			m.Render ();
		}

	}

	public void Delete() {
		// Go through links and delete all linerenderers
		for (int j = 0; j < points.Count; j++) {
			PointMass temp = (PointMass) points [j];
			ArrayList links = temp.links;
			for (int i = 0; i < links.Count; i++) {
				PointLink link = (PointLink) links [i];
				link.Destroy ();
			}
		}
		// Destroy all spheres
		foreach (PointMass p in points) {
			GameObject sphere = p.pointrender;
			if (sphere != null )
				Destroy (sphere);
		}
		// Destroy this
		Destroy (gameObject);
	}

	// Unhinge all the points in this goat
	public void Unhinge() {
		foreach (PointMass p in points) {
			p.Unhinge ();
		}
	}

}

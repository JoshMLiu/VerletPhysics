using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {

	public Vector3 pos;
	public Vector3 velocity;

	// Game properties
	Properties props;

	public float radius = 0.14f;
	// coefficient of reconstitution
	float bounceback = 0.7f;
	// how much of the previous velocity to keep after a collision
	float momentumkeep = 0.05f;
	// countdown for when a ball is stationary
	float alivetime;
	GameObject sphere;
	CreateMountain mountain;
	// how many velocity change attempts before calling the ball stationary
	int trycounter;
	// to check for goat collisions
	GameObject goatspawner;

  	void Start () {

		GameObject gi = GameObject.Find ("GameInitializer");
 		props = (Properties) gi.transform.GetComponent ("Properties");

		sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.localScale = new Vector3 (2*radius, 2*radius, 2*radius);
		SphereCollider col = sphere.GetComponent<SphereCollider> ();
		col.isTrigger = true;
		sphere.AddComponent<WallHit> ();
		sphere.AddComponent<Rigidbody> ();
		sphere.transform.parent = transform;
		alivetime = 2.0f;

		mountain = (CreateMountain) gi.transform.GetComponent ("CreateMountain");
		trycounter = 0;
		goatspawner = GameObject.Find ("GoatSpawner");

	}
		
	// Update is called once per frame
 	void Update () {

		// ball is not moving
		if (velocity.x == 0 && velocity.y == 0) {
			sphere.transform.position = pos;
			// wait x seconds before deleting
			alivetime -= Time.deltaTime;
			if (alivetime < 0) {
				Destroy (gameObject);
			}
			return;
		}

		// Calculate a new position with velocity, airresistance, gravity, wind
		TriangleStruct[] lefttriangles = mountain.lefttriangles;
		TriangleStruct[] righttriangles = mountain.righttriangles;

		float gravity = props.GetGravity ();
		float airres = props.GetAirRes ();
		float windspeed = props.GetWindforce ();

		velocity = velocity + new Vector3 (0, gravity, 0);

		if (velocity.x > 0) {
			velocity = velocity + new Vector3 (-airres, 0, 0);
		} else {
			velocity = velocity + new Vector3 (airres, 0, 0);
		}

		velocity = velocity + new Vector3 (windspeed, 0, 0);

		Vector3 newpos = pos + velocity;

		// Collision Detection + Response

		// Check for goats
		Transform goats = goatspawner.transform;
		// Get each goat instance
		foreach (Transform goat in goats) {
			GoatPoints gp = (GoatPoints) goat.GetComponent ("GoatPoints");

			// surround each goat by a generous sphere bounding box and check that first 
			float r = gp.radius;
			PointMass c = gp.center;
			if ((c.position - (new Vector2 (pos.x, pos.y))).magnitude > r)
				continue;

			// Check each point in the goat for a collision
			ArrayList points = gp.points;
			for (int i = 0; i < points.Count; i++) {
				PointMass p = (PointMass) points [i];
				if (HitGoat (p)) {
					// If there is a collision, delete this ball and unanchor the goat (giving it the ball's velocity)
					Vector2 velocity2d = new Vector2 (velocity.x, velocity.y);
					gp.Unhinge ();
					p.GiveVelocity (velocity2d);
					Destroy (gameObject);
				}
			}
		}

		// bounding boxes for mountains 
		if (pos.y - radius <= 2.0f) {
			// left
			if (pos.x + radius >= -5.0f && pos.x - radius <= -1.0f) {
				// left mountain triangles
				for (int i = 0; i < lefttriangles.Length; i++) {

					TriangleStruct lefttri = lefttriangles [i];

					if (lefttri.left == lefttri.right) {
						continue;
					}
					// If there is a collision, change the velocity and recursively calulate a new position (up to 'counter' times)
					if (lefttri.SphereCollides(newpos, radius)) {
						ChangeVelocity(lefttri.getNormal());
						trycounter++;
						Update ();
						return;
					}

				}
			}
			// right
			if (pos.x + radius >= 1.0f && pos.x - radius <= 5.0f) {
				// right mountain triangles
				for (int i = 0; i < righttriangles.Length; i++) {

					TriangleStruct righttri = righttriangles [i];

					if (righttri.left == righttri.right) {
						continue;
					}
					if (righttri.SphereCollides(newpos, radius)) {
						ChangeVelocity(righttri.getNormal());
						trycounter++;
						Update ();
						return;
					}

				}
			}
			// middle
			if (pos.x + radius >= -1.0f && pos.x - radius <= 1.0f) {
				// top of mountain
				TriangleStruct toptriangle = mountain.toptriangle;
				if (toptriangle.SphereCollides (newpos, radius)) {
					ChangeVelocity (toptriangle.getNormal ());
					trycounter++;
					Update ();
					return;
				}
			}
		}
			
		// Update Position
		pos = newpos;
		sphere.transform.position = pos;
		trycounter = 0;

	}

	// Calculates whether this ball is in the goats bounding box
	bool HitGoat(PointMass p) {
		Vector2 goatpos = p.position;
		Vector2 dist = (new Vector2(pos.x, pos.y)) - goatpos;
		if (dist.magnitude <= radius)
			return true;
		return false;
	}
		
	void ChangeVelocity(Vector2 normal) {

		// Get reflection on surface normal
		Vector2 tempvelocity = new Vector2 (velocity.x, velocity.y);
		float tempdot = 2 * Vector2.Dot (tempvelocity, normal);

		// reflecting, dampening the velocity by bounceback, keeping a scaled factor of the original velocity as momentum.
		Vector2 newvelocity = tempvelocity - Vector2.Scale (new Vector2 (tempdot, tempdot), normal);
		newvelocity = Vector2.Scale (new Vector2 (bounceback, bounceback), newvelocity);
		newvelocity = newvelocity + Vector2.Scale (new Vector2 (momentumkeep, momentumkeep), velocity);

		// This counter value sort of simulates friction by simluating the number of new velocities to try, with a higher value being more slippery (but also lags the game when its too high...)
		if (trycounter < 100) {
			velocity = new Vector3 (newvelocity.x, newvelocity.y, 0);
		} else {
			velocity = new Vector3 ();
		}

	}
								
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriangleStruct {

	// Three points
	public Vector2 basepoint;
	public Vector2 left;
	public Vector2 right;

	// Directons of triangle lines for collision 
	Ray2D basetoleft;
	Ray2D basetoright;
	Ray2D lefttoright;

	float btlmax;
	float btrmax;
	float ltrmax;

	// Normal is only for the outer edge because we know this is the only edge exposed.
	Vector2 normal;

	public TriangleStruct(Vector3 b, Vector3 l, Vector3 r) {

		// vector math to get rays representing the triangle lines
		basepoint = new Vector2(b.x, b.y);
		left = new Vector2(l.x, l.y);
		right = new Vector2(r.x, r.y);

		Vector2 btl = left - basepoint;
		Vector2 btr = right - basepoint;
		Vector2 ltr = right - left;

		basetoleft = new Ray2D ();
		basetoleft.origin = basepoint;
		basetoleft.direction = btl.normalized;
		btlmax = btl.magnitude;

		basetoright = new Ray2D ();
		basetoright.origin = basepoint;
		basetoright.direction = btr.normalized;
		btrmax = btr.magnitude;

		lefttoright = new Ray2D ();
		lefttoright.origin = left;
		lefttoright.direction = ltr.normalized;
		ltrmax = ltr.magnitude;

		normal = new Vector2 (ltr.y, -ltr.x);
		normal.Normalize ();
	}
		
	public Vector2 getNormal() {
		return normal;
	}

	// Minkowski sum surrounding triangle with spheres
	public bool SphereCollides (Vector3 c, float radius) {

		Vector2 center = new Vector2 (c.x, c.y);

		// Only need to test one edge because we know that it is the only edge exposed. 
		// Uncomment these for the other two edges (but you also need to calculate the 2 other normals)

		/*float btld = 0;
		while (btld <= btlmax) {
			Vector2 btlpoint = basetoleft.GetPoint (btld);
			if ((center - btlpoint).magnitude < radius) {
				return true;
			}
			btld += radius;
		}

		float btrd = 0;
		while (btrd <= btrmax) {
			Vector2 btrpoint = basetoright.GetPoint (btrd);
			if ((center - btrpoint).magnitude < radius) {
				return true;
			}
			btrd += radius;
		}*/

		// Outer edge
		float ltrd = 0;
		while (ltrd <= ltrmax) {
			Vector2 ltrpoint = lefttoright.GetPoint (ltrd);
			if ((center - ltrpoint).magnitude < radius) {
				return true;
			}
			ltrd += radius;
		}

		// Didn't hit any spheres
		return false;
	}
		
	// sign test
	float sign (Vector2 p1, Vector2 p2, Vector2 p3) {
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}

	// check if a point is in a triangle
	public bool PointInTriangle (Vector2 pt)
	{
		bool b1, b2, b3;

		b1 = sign(pt, basepoint, left) < 0.0f;
		b2 = sign(pt, left, right) < 0.0f;
		b3 = sign(pt, right, basepoint) < 0.0f;

		return ((b1 == b2) && (b2 == b3));
	}

	public Vector2 Relocate(Vector2 p) {

		// use projection to find a point to relocate the point outside of the triangle in a proper location

		Vector2 pointline = p - left;

		//lefttoright
		Vector3 raydir3d = new Vector3(pointline.x, pointline.y, 0);
		Vector3 leftright3d = new Vector3(lefttoright.direction.x, lefttoright.direction.y, 0);

		Vector3 proj = Vector3.Project (raydir3d, leftright3d);
		Vector2 proj2d = new Vector2 (proj.x, proj.y);

		Vector2 newpoint = left + proj2d;

		// offset to place it a little bit outside of the triangle in the direction of the edge normal
		newpoint = newpoint + Vector2.Scale (new Vector2 (0.000001f, 0.000001f), normal);
		return newpoint;
	}

}

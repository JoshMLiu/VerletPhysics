using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CreateMountain : MonoBehaviour {

	// Middle plains
	ArrayList middlepoints = new ArrayList();
	public TriangleStruct toptriangle;

	// Must be at least 1, increasing gives finer detail on mountain sides
	static readonly int SUBDIVISIONS = 6;

	Mesh leftmesh;
	Mesh rightmesh;

	// triangles that represent each mountain side, all sharing a base point inside the mountain (fan of triangles).
	public TriangleStruct[] lefttriangles;
	public TriangleStruct[] righttriangles;

	void Start () {

		leftmesh = new Mesh ();
		rightmesh = new Mesh (); 

		ArrayList leftpoints = new ArrayList();
		ArrayList rightpoints = new ArrayList();

		// Add middle rectangle
		middlepoints.Add (new Vector2 (-1f, -4f));
		middlepoints.Add (new Vector2 (1f, -4f));
		middlepoints.Add (new Vector2 (-1f, 1.5f));
		middlepoints.Add (new Vector2 (1f, 1.5f));

		// Create triangle that represents the middle
		Vector3[] toppoints = new Vector3[2];
		toppoints [0] = new Vector2 (-1f, 1.5f);
		toppoints [1] = new Vector2 (1f, 1.5f);
		toptriangle = new TriangleStruct (new Vector3(0, -1, 0), toppoints [0], toppoints [1]);

		// Add a straight line where the mountain slope will be on each side
		leftpoints.Add (new Vector2 (-5f, -4f));
		leftpoints.Add (new Vector2 (-1f, 1.5f));

		rightpoints.Add (new Vector2 (1f, 1.5f));
		rightpoints.Add (new Vector2 (5f, -4f));

		// Subdivide mountain sides
		for (int i = 0; i < SUBDIVISIONS; i++) {

			// Left
			ArrayList templeftpoints = new ArrayList ();
			for (int j = 0; j < leftpoints.Count - 1; j++) {

				// get a midpoint and randomly scale by normal (based on line length)
				Vector2 leftfirstpoint = (Vector2) leftpoints [j];
				Vector2 leftsecondpoint = (Vector2) leftpoints [j + 1];
				Vector2 leftline = leftsecondpoint - leftfirstpoint; 
				Vector2 leftnormal = new Vector2 (-leftline.y, leftline.x);
				leftnormal = leftnormal.normalized;
				Vector2 leftmidpoint = Vector2.Scale(leftfirstpoint, new Vector2(0.5f, 0.5f)) + Vector2.Scale(leftsecondpoint, new Vector2(0.5f, 0.5f));
				float leftnorm = leftline.magnitude;
				float leftbumpscale = leftnorm / 5;
				float leftbumpoffset = UnityEngine.Random.Range (-leftbumpscale*(0.8f), leftbumpscale);
				Vector2 newleftpoint = leftmidpoint + (Vector2.Scale (leftnormal, new Vector2(leftbumpoffset, leftbumpoffset)));

				templeftpoints.Add (leftfirstpoint);
				templeftpoints.Add (newleftpoint);
				templeftpoints.Add (leftsecondpoint);
			}
			leftpoints = templeftpoints;

			// Right side
			ArrayList temprightpoints = new ArrayList ();
			for (int j = 0; j < rightpoints.Count - 1; j++) {

				Vector2 rightfirstpoint = (Vector2) rightpoints [j];
				Vector2 rightsecondpoint = (Vector2) rightpoints [j + 1];
				Vector2 rightline = rightsecondpoint - rightfirstpoint; 
				Vector2 rightnormal = new Vector2 (-rightline.y, rightline.x);
				rightnormal = rightnormal.normalized;
				Vector2 rightmidpoint = Vector2.Scale(rightfirstpoint, new Vector2(0.5f, 0.5f)) + Vector2.Scale(rightsecondpoint, new Vector2(0.5f, 0.5f));
				float rightnorm = rightline.magnitude;
				float rightbumpscale = rightnorm / 5;
				float rightbumpoffset = UnityEngine.Random.Range (-rightbumpscale*(0.8f), rightbumpscale);
				Vector2 newrightpoint = rightmidpoint + (Vector2.Scale (rightnormal, new Vector2(rightbumpoffset, rightbumpoffset)));

				temprightpoints.Add (rightfirstpoint);
				temprightpoints.Add (newrightpoint);
				temprightpoints.Add (rightsecondpoint);
			}
			rightpoints = temprightpoints;
		
		}

		// Create lists of triangles all with the same base poin. Triangles (b, 1, 2), (b, 2, 3), (b, 3, 4) etc...
		Vector2 leftbase = new Vector3 (-1f, -4f, 0);
		Vector2 rightbase = new Vector3 (1f, -4f, 0);

		Vector2[] templeft = Array.ConvertAll(leftpoints.ToArray(), new Converter<object, Vector2>(ObjToVector));
		Vector2[] tempright = Array.ConvertAll(rightpoints.ToArray(), new Converter<object, Vector2>(ObjToVector));

		// Converting to 3d points
		Vector3[] leftvertices = new Vector3[templeft.Length + 1];
		leftvertices [0] = leftbase;
		for (int i = 0; i < templeft.Length; i++) {
			leftvertices[i+1] = new Vector3(templeft[i].x, templeft[i].y, 0);
		}

		Vector3[] rightvertices = new Vector3[tempright.Length + 1];
		rightvertices [0] = rightbase;
		for (int i = 0; i < tempright.Length; i++) {
			rightvertices[i+1] = new Vector3(tempright[i].x, tempright[i].y, 0);
		}

		List<int> leftindexlist = new List<int>();
		List<int> rightindexlist = new List<int>(); 

		for (int i = 1; i <= templeft.Length - 1; i++) {
			leftindexlist.Add (0);
			leftindexlist.Add (i);
			leftindexlist.Add (i + 1);
		}

		for (int i = 1; i <= tempright.Length - 1; i++) {
			rightindexlist.Add (0);
			rightindexlist.Add (i);
			rightindexlist.Add (i + 1);
		}
			
		int[] leftindices = leftindexlist.ToArray();
		int[] rightindices = rightindexlist.ToArray();

		// Creating the meshes
		leftmesh.vertices = leftvertices;
		leftmesh.triangles = leftindices;
		leftmesh.RecalculateNormals();
		leftmesh.RecalculateBounds();

		rightmesh.vertices = rightvertices;
		rightmesh.triangles = rightindices;
		rightmesh.RecalculateNormals();
		rightmesh.RecalculateBounds();

		// Add Meshes to Gameobjects
		GameObject leftmountain = GameObject.Find("MountainLeft");
		MeshFilter leftfilter = leftmountain.transform.GetComponent<MeshFilter> ();
		leftfilter.mesh = leftmesh;

		GameObject rightmountain = GameObject.Find("MountainRight");
		MeshFilter rightfilter = rightmountain.transform.GetComponent<MeshFilter> ();
		rightfilter.mesh = rightmesh;

		GameObject middlemountain = GameObject.Find("MountainMiddle");
		middlemountain.transform.position = new Vector3 (0, -1.25f, 0);
		middlemountain.transform.localScale = new Vector3 (2f, 5.5f, 1f);

		// Creating lists of Triangle Structures
		int[] lefttriindices = leftmesh.triangles;
		Vector3[] lefttrivertices = leftmesh.vertices;
		lefttriangles = new TriangleStruct[leftindices.Length/3];
		for (int i = 0; i < lefttriangles.Length; i++) {
			Vector3 leftbasepoint = leftvertices [leftindices [3*i]];
			Vector3 leftpointone = leftvertices [leftindices [3*i + 1]];
			Vector3 leftpointtwo = leftvertices [leftindices [3*i + 2]];
			TriangleStruct leftt = new TriangleStruct (leftbasepoint, leftpointone, leftpointtwo);
			lefttriangles [i] = leftt;
		}

		int[] righttriindices = rightmesh.triangles;
		Vector3[] righttrivertices = rightmesh.vertices;
		righttriangles = new TriangleStruct[rightindices.Length/3];
		for (int i = 0; i < righttriangles.Length; i++) {
			Vector3 rightbasepoint = rightvertices [rightindices [3*i]];
			Vector3 rightpointone = rightvertices [rightindices [3*i + 1]];
			Vector3 rightpointtwo = rightvertices [rightindices [3*i + 2]];
			TriangleStruct rightt = new TriangleStruct (rightbasepoint, rightpointone, rightpointtwo);
			righttriangles [i] = rightt;
		}

	}

	public Mesh LeftMesh() {
		return leftmesh;
	}

	public Mesh RightMesh() {
		return rightmesh;
	}

	public ArrayList MiddlePoints() {
		return middlepoints;
	}

	public static Vector2 ObjToVector(object o) {
		return (Vector2) o;
	}
		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidePath : MonoBehaviour {

	public Vector3[] pathVectors { get; private set; }

	EdgeCollider2D cd;
	Rigidbody2D rb;
	LineRenderer rend;

	int numVectors;

	// Use this for initialization
	void Start () {
		rend = GetComponent<LineRenderer>();
		pathVectors = new Vector3[rend.positionCount];
		numVectors = rend.GetPositions(pathVectors);

		// Set up collider and its requisite rigidbody
		rb = gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		rb.bodyType = RigidbodyType2D.Kinematic;
		cd = gameObject.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
		cd.points = ConvertToVector2Array(pathVectors);
		cd.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	Vector2[] ConvertToVector2Array (Vector3[] v3) {
		Vector2 [] v2 = new Vector2[v3.Length];

		for (int i = 0; i <  v3.Length; i++){
			Vector3 tempV3 = v3[i];
			v2[i] = new Vector2(tempV3.x, tempV3.y);
		}

		return v2;
 	}
}

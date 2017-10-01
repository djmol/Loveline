using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidePath : MonoBehaviour {

	public int drawOrder = 0;
	public Vector3[] pathVectors { get; private set; }
	public Vector3 maxX { get; private set; }
	public bool unidirectional = true;
	
	EdgeCollider2D cd;
	Rigidbody2D rb;
	LineRenderer rend;

	LTSpline pathSpline;
	public GameObject pathShine;
	public float shineSpeed;
	GameObject shine;

	int numVectors;

	// Use this for initialization
	void Start () {
		rend = GetComponent<LineRenderer>();
		pathVectors = new Vector3[rend.positionCount];
		numVectors = rend.GetPositions(pathVectors);

		// Set up collider and its requisite rigidbody
		rb = gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		rb.bodyType = RigidbodyType2D.Kinematic;
		GameObject cdGO = new GameObject("Collider");
		cdGO.layer = LayerMask.NameToLayer("Path");
		cdGO.transform.parent = gameObject.transform;
		cd = cdGO.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
		cd.points = ConvertToVector2Array(pathVectors);
		cd.isTrigger = true;
		pathSpline = SplineHelper.CreateSpline(pathVectors);

		rend.sortingLayerName = "Character";

		// Get position with largest x value (used to determine whether path is still on screen)
		maxX = GetMaximumXVector();
		LevelManager lm = GameObject.FindWithTag("GameController").GetComponent<LevelManager>();
		lm.paths.Add(this);

		if (unidirectional) {
			shine = new GameObject("Shine");
			GameObject shineRendGO = (GameObject)Instantiate(Resources.Load("Prefabs/RoundShine"));
			SpriteRenderer shineRend = shineRendGO.GetComponent<SpriteRenderer>();
			shineRend.sortingLayerName = "Character";
			shineRend.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, .75f);
			shine.transform.parent = gameObject.transform;
			shineRendGO.transform.parent = shine.transform;
			shineRendGO.transform.position = new Vector3(shine.transform.position.x, shine.transform.position.y, shine.transform.position.z + 1);
			ShinePathDirection();
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	void ShinePathDirection () {
		LeanTween.moveSpline(shine, pathSpline, 10f).setSpeed(5f).setOnComplete(ShinePathDirection);
	}

	Vector2[] ConvertToVector2Array (Vector3[] v3) {
		Vector2 [] v2 = new Vector2[v3.Length];

		for (int i = 0; i <  v3.Length; i++){
			Vector3 tempV3 = v3[i];
			v2[i] = new Vector2(tempV3.x, tempV3.y);
		}

		return v2;
 	}

	Vector3 GetMaximumXVector () {
		Vector3 maxVector = new Vector3(float.MinValue, 0);

		foreach (Vector3 vec in pathVectors) {
			if (vec.x > maxVector.x)
				maxVector = vec;
		}

		return maxVector;
	}
}
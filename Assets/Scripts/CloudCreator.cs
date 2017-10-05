using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCreator : MonoBehaviour {

	public Transform[] clouds;
	public float cloudDensity;
	LevelManager lm;
	float levelSize;

	// Use this for initialization
	void Start () {
		lm = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
		levelSize = lm.endPoint.position.x - lm.startPoint.position.x;

		GameObject[] clouds = new GameObject[]{ 
			(GameObject)Resources.Load("Prefabs/Cloud1"),
			(GameObject)Resources.Load("Prefabs/Cloud2"),
			(GameObject)Resources.Load("Prefabs/Cloud3")
		};

		// TODO: Get vertical level bounds, and then randomize positions
		for (float x = lm.startPoint.position.x; x < lm.endPoint.position.x; x += cloudDensity * Random.Range(1f, 2f)) {
			GameObject cl = Instantiate(clouds[Random.Range(0,3)], new Vector3(x, Random.Range(lm.lowerYBound, lm.upperYBound), 4), Quaternion.identity, transform);
			cl.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}

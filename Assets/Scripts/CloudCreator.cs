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
	}
	
	// Update is called once per frame
	void Update () {
		// TODO: Get vertical level bounds, and then randomize positions
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public Transform startPoint;
	public Transform endPoint;
	public float unitTravelTime; // The average amount of time it takes for the camera to travel one unit
	Camera cam;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

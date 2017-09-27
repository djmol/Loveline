using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public Transform startPoint;
	public Transform endPoint;
	public float unitTravelTime; // The average amount of time it takes for the camera to travel one unit
	Camera cam;
	public List<RidePath> paths { get; private set; }
	List<RidePath> pathsToDelete;

	float checkForDeadPaths = 5f;
	float nextCheckForDeathPaths;

	void Awake() {
		paths = new List<RidePath>();
		pathsToDelete = new List<RidePath>();
	}

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		nextCheckForDeathPaths = checkForDeadPaths;
	}
	
	// Update is called once per frame
	void Update () {
		// TODO: This doesn't work, and replace the - 11f with half of the screen width
		if (Time.time > nextCheckForDeathPaths) {
			nextCheckForDeathPaths += checkForDeadPaths;
			foreach (RidePath path in paths) {
				if (path.maxX.x < cam.transform.position.x - 11f) { 
					pathsToDelete.Add(path);
				}
			}

			if (pathsToDelete.Count > 0) {
				foreach (RidePath path in pathsToDelete) {
					Destroy(path);
					paths.Remove(path);
				}

				pathsToDelete.Clear();
			}
		}

		
	}
}

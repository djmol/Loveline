using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public Transform startPoint;
	public Transform endPoint;
	public float upperXBound { get; private set; }
	public float lowerXBound { get; private set; }
	public float upperYBound { get; private set; }
	public float lowerYBound { get; private set; }
	public float unitTravelTime; // The average amount of time it takes for the camera to travel one unit
	public List<GameObject> riders { get; private set; }
	public List<RidePath> paths { get; private set; }
	public bool dead { get; private set; }
	public int addLifePoints = 0;
	public int maxAddLifePoints = 8;

	List<RidePath> pathsToDelete;
	float checkForDeadPaths = 5f;
	float nextCheckForDeathPaths;
	DrawListener dl;
	Camera cam;
	CameraController camController;
	float deathBoundX;
	float deathBoundY;
	UnityEngine.PostProcessing.PostProcessingBehaviour postProc;
	UnityEngine.PostProcessing.PostProcessingProfile deathProfile;

	void Awake() {
		riders = new List<GameObject>();
		paths = new List<RidePath>();
		pathsToDelete = new List<RidePath>();
	}

	// Use this for initialization
	void Start () {
		dl = GameObject.FindGameObjectWithTag("DrawListener").GetComponent<DrawListener>();
		cam = Camera.main;
		camController = cam.gameObject.GetComponent<CameraController>();
		nextCheckForDeathPaths = checkForDeadPaths;
		riders.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		postProc = cam.gameObject.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
		deathProfile = (UnityEngine.PostProcessing.PostProcessingProfile)Instantiate(Resources.Load("Profiles/DeathProfile"));

		// Determine camera bounds
		upperXBound = cam.gameObject.transform.position.x + (cam.orthographicSize * Screen.width / Screen.height);
		lowerXBound = cam.gameObject.transform.position.x - (cam.orthographicSize * Screen.width / Screen.height);
		upperYBound = cam.gameObject.transform.position.y + cam.orthographicSize;
		lowerYBound = cam.gameObject.transform.position.y - cam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		if (dead) {
			if (Input.GetMouseButtonUp(0)) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			}
		}

		// Update camera bounds
		upperXBound = cam.gameObject.transform.position.x + (cam.orthographicSize * Screen.width / Screen.height);
		lowerXBound = cam.gameObject.transform.position.x - (cam.orthographicSize * Screen.width / Screen.height);
		upperYBound = cam.gameObject.transform.position.y + cam.orthographicSize;
		lowerYBound = cam.gameObject.transform.position.y - cam.orthographicSize;

		// Update death bounds
		deathBoundX = lowerXBound;
		deathBoundY = lowerYBound;		

		// Watch for deaths
		foreach (GameObject rider in riders) {
			if (rider.GetComponent<PathRider>().mainRider) {
				float trueX = rider.transform.position.x + rider.GetComponent<BoxCollider2D>().size.x / 2;
				float trueY = rider.transform.position.y + rider.GetComponent<BoxCollider2D>().size.y / 2;
				if (trueX < deathBoundX || trueY < deathBoundY) {
					dl.gameObject.SetActive(false);
					rider.SetActive(false);
					camController.StopCamera();
					postProc.profile = deathProfile;
					postProc.enabled = true;
					dead = true;
				}
			}
		}

		// Check for and remove dead paths
		if (Time.time > nextCheckForDeathPaths) {
			nextCheckForDeathPaths += checkForDeadPaths;
			foreach (RidePath path in paths) {
				if (path.maxX.x < deathBoundX) { 
					pathsToDelete.Add(path);
				}
			}

			if (pathsToDelete.Count > 0) {
				foreach (RidePath path in pathsToDelete) {
					Destroy(path.gameObject);
					paths.Remove(path);
				}
				pathsToDelete.Clear();
			}
		}		
	}
}

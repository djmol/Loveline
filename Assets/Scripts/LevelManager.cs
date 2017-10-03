using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public Transform startPoint;
	public Transform endPoint;
	public float unitTravelTime; // The average amount of time it takes for the camera to travel one unit
	public List<GameObject> riders { get; private set; }
	public List<RidePath> paths { get; private set; }
	public bool dead { get; private set; }
	public float addLifePoints = 0f;
	public float maxAddLifePoints = 8f;

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
	}
	
	// Update is called once per frame
	void Update () {
		if (dead) {
			if (Input.GetMouseButtonUp(0)) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			}
		}

		// Update death bounds
		deathBoundX = cam.gameObject.transform.position.x - (cam.orthographicSize * Screen.width / Screen.height);
		deathBoundY = cam.gameObject.transform.position.y - cam.orthographicSize;

		// Watch for deaths
		foreach (GameObject rider in riders) {
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

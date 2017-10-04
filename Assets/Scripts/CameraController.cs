using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Level information
	LevelManager levelManager;
	LTSpline camSpline;

	// Camera movement
	int start = -1;

	// Use this for initialization
	void Start () {
		levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
		transform.position = levelManager.startPoint.position;
		camSpline = new LTSpline(new Vector3[]{
			levelManager.startPoint.position,
			levelManager.startPoint.position,
			levelManager.endPoint.position,
			levelManager.endPoint.position
		});
		start = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (start == 1) {
			LeanTween.moveSpline(gameObject, camSpline, levelManager.unitTravelTime * (levelManager.endPoint.position.x - levelManager.startPoint.position.x));
			start = 0;
		}
	}

	public void StopCamera() {
		LeanTween.cancel(gameObject);
	}
}

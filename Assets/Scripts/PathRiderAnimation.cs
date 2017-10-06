using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathRider))]
public class PathRiderAnimation : MonoBehaviour {

	public PathRider rider;
	public SpriteRenderer rend;

	float wobbleAngle = 15f;
	float wobbleTime = 1f;
	bool wobbling = false;
	bool exitingWobble = false;

	// Use this for initialization
	void Start () {
		rend = rider.GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (rider.onPath && !wobbling) {
			EnterWobble();
		} else if (!rider.onPath && wobbling && !exitingWobble) {
			ExitWobble();
		}
	}

	void EnterWobble () {
		float angle = 0f;
		if (rend.gameObject.transform.eulerAngles.z <= wobbleAngle) {
			angle = wobbleAngle - rend.gameObject.transform.eulerAngles.z;
		} else {
			angle = (360 - rend.gameObject.transform.eulerAngles.z) + wobbleAngle;
		}
		// TODO: Normalize the wobble entry/exit times
		LeanTween.rotateAround(rend.gameObject, new Vector3(0,0,1), angle, wobbleTime / 2).setEaseInOutQuad().setOnComplete(FullWobble);
		wobbling = true;
		exitingWobble = false;
	}

	void FullWobble () {
		LeanTween.rotateAround(rend.gameObject, new Vector3(0,0,1), -wobbleAngle * 2, wobbleTime).setLoopPingPong().setEaseInOutQuad();
	}

	void ExitWobble () {
		LeanTween.cancel(rend.gameObject);
		float angleBack = 0f;
		if (rend.gameObject.transform.eulerAngles.z <= wobbleAngle) {
			angleBack = -rend.gameObject.transform.eulerAngles.z;
		} else {
			Debug.Log("dismounting at z = " + rend.gameObject.transform.eulerAngles.z);
			angleBack = 360 - rend.gameObject.transform.eulerAngles.z;
		}
		
		LeanTween.rotateAround(rend.gameObject, new Vector3(0,0,1), angleBack, wobbleTime / 2).setOnComplete(OnWobbleExit);
		exitingWobble = true;
	}

	void OnWobbleExit () {
		exitingWobble = false;
		wobbling = false;
	}
}

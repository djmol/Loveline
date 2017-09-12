using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRider : MonoBehaviour {

	public float gravity = .001f;
	public float maxFall = 1.5f;
	public float onPathSpeed = 2f;
	public float offPathSpeed = 2.5f;

	public LineRenderer currentPath;
	Vector3[] currentPathVectors;

	float startTime = 1f;
	float startTimer = 0f;
	bool started = false;

	bool onPath;
	Vector2 velocity;
	Collider2D cd;
	Rect box;

	// Raycasting
	int vRays = 4;
	RaycastHit2D closestHitInfo;

	// Use this for initialization
	void Start () {
		cd = GetComponent<BoxCollider2D>();
		velocity = new Vector2(offPathSpeed, 0f);
		currentPathVectors = new Vector3[currentPath.positionCount];
		currentPath.GetPositions(currentPathVectors);

		//gameObject.transform.position = currentPathVectors[0];
	}
	
	void Update() {
		/*startTimer += Time.deltaTime;
		if (startTimer >= startTime && !started) {
			started = true;
			RidePath(currentPathVectors);
		}*/
		started = true;
		/*if (!onPath)
			LeanTween.moveSppath(gameObject, positions, 10f).setSpeed(2f).setOnComplete(LeavePath);*/
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate() {
		// For convenience
		box = new Rect(
			cd.bounds.min.x,
			cd.bounds.min.y,
			cd.bounds.size.x,
			cd.bounds.size.y
		);

		// Apply gravity
		if (!onPath) {
			velocity = new Vector2(offPathSpeed, Mathf.Max(velocity.y - gravity, -maxFall));
		} 
		// Ride path
		else {
			//velocity = new Vector2(onPathSpeed, 0f);
		}

		// If not on path, check for path below
		if (!onPath) {
			// Determine first and last rays
			Vector2 minRay = new Vector2(box.xMin, box.center.y);
			Vector2 maxRay = new Vector2(box.xMax, box.center.y);	

			// Calculate ray distance (current fall speed)
			float rayDistance = box.height / 2 + Mathf.Abs(velocity.y * Time.deltaTime);

			// Check below for path
			RaycastHit2D[] hitInfo = new RaycastHit2D[vRays];
			bool hit = false;
			float closestHit = float.MaxValue;
			int closestHitIndex = 0;
			for (int i = 0; i < vRays; i++) {
				// Create and cast ray
				float lerpDistance = (float)i / (float)(vRays - 1);
				Vector2 rayOrigin = Vector2.Lerp(minRay, maxRay, lerpDistance);
				Ray2D ray = new Ray2D(rayOrigin, Vector2.down);
				hitInfo[i] = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance);
				Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan, 1f);
				// Check raycast results and keep track of closest path hit
				if (hitInfo[i].fraction > 0) {
					hit = true;
					if (hitInfo[i].fraction < closestHit) {
						closestHit = hitInfo[i].fraction;
						closestHitIndex = i;
						closestHitInfo = hitInfo[i];
					}
				}
			}

			// If player hits path, snap to the nearest path hit
			if (hit) {
				Debug.Log("hit!!!");
				RidePath path = closestHitInfo.collider.gameObject.GetComponent<RidePath>();
				PlaceSelfInPath(closestHitInfo.point, path.pathVectors);
			}
		}
	}

	/// <summary>
	/// LateUpdate is called every frame, if the Behaviour is enabled.
	/// It is called after all Update functions have been called.
	/// </summary>
	void LateUpdate() {
		// Move rider
		if (started) {
			if (onPath) {
				
			} else {
				transform.Translate(velocity * Time.deltaTime);
			}
		}
	}

	void PlaceSelfInPath(Vector3 pos, Vector3[] path) {
		// Find closest point in path
		float shortestDistance = float.MaxValue;
		int shortest = -1;
		for (int i = 0; i < path.Length; i++) {
			float distance = Vector3.Distance(pos, path[i]);
			if (distance < shortestDistance) {
				shortestDistance = distance;
				shortest = i;
			}
		}

		// Create new path starting from boarding point
		int newPathLength = path.Length - shortest;
		
		// LT requires 4 or more points in a spline path!
		if (newPathLength > 3) {
			Vector3[] newPath = new Vector3[newPathLength];
			for (int i = 0; i < newPathLength; i++) {
				newPath[i] = path[shortest + i];
			}
			
			// Ride newly created path
			RidePath(newPath);
		}
	}

	void RidePath(Vector3[] path) {
		onPath = true;
		LeanTween.moveSpline(gameObject, path, 10f).setSpeed(onPathSpeed).setOnComplete(LeavePath);
	}

	void LeavePath() {
		LeanTween.cancel(gameObject);
		onPath = false;
		velocity = new Vector2(offPathSpeed, 0f);
	}

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI() {
		GUI.Box(new Rect(5,5,150,30), "velocity: " + velocity);
		GUI.Box(new Rect(5,40,50,30), onPath.ToString());
	}
}

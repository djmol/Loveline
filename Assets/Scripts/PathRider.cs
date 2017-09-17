using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRider : MonoBehaviour {

	public float gravity = .001f;
	public float maxFall = 1.5f;
	public float onPathSpeed = 2f;
	public float offPathSpeed = 2.5f;

	RidePath currentPath = null;
	Vector3[] currentPathVectors = null;

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
	}
	
	void Update() {
		/*startTimer += Time.deltaTime;
		if (startTimer >= startTime && !started) {
			started = true;
			RidePath(currentPathVectors);
		}*/
		started = true;
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
			velocity = new Vector2(velocity.x, Mathf.Max(velocity.y - gravity, -maxFall));
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

			// Calculate ray distance (current fall speed) and direction
			float rayDistance = box.height / 2 + Mathf.Abs(velocity.y * Time.deltaTime);

			// Cast the ray to check for path
			RaycastHit2D[] hitInfo = new RaycastHit2D[vRays];
			bool hit = false;
			float closestHit = float.MaxValue;
			int closestHitIndex = 0;
			for (int i = 0; i < vRays; i++) {
				// Create and cast ray
				float lerpDistance = (float)i / (float)(vRays - 1);
				Vector2 rayOrigin = Vector2.Lerp(minRay, maxRay, lerpDistance);
				Ray2D ray = new Ray2D(rayOrigin, velocity.normalized);
				hitInfo[i] = Physics2D.Raycast(rayOrigin, velocity.normalized, rayDistance);
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
				RidePath path = closestHitInfo.collider.gameObject.GetComponent<RidePath>();
				if (path)
					PlaceSelfInPath(closestHitInfo.point, path);
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

	void PlaceSelfInPath(Vector3 pos, RidePath path) {
		// Find closest point in path
		Vector3[] pathVectors = path.pathVectors;
		float shortestDistance = float.MaxValue;
		int shortest = -1;
		for (int i = 0; i < pathVectors.Length; i++) {
			float distance = Vector3.Distance(pos, pathVectors[i]);
			if (distance < shortestDistance) {
				shortestDistance = distance;
				shortest = i;
			}
		}

		// Get length of partial path (+ 1 as our point of contact will be the starting point)
		int partialPathLength = (pathVectors.Length - shortest) + 1;
		
		// LeanTween requires 4 or more points in a spline path!
		if (partialPathLength > 3) {
			// Ride partial path
			RidePath(path, shortest, pos);
		}
	}

	void RidePath(RidePath path, int start = 0, Vector3? startVector = null) {
		Vector3[] ridePath = new Vector3[0];

		// Trim beginning of vector array if starting index is not 0...
		if (start != 0) {
			int partialPathLength = path.pathVectors.Length - start;
			int startIndex = 0;
			// Change starting vector if provided
			if (startVector != null && startVector.GetType().Equals(typeof(Vector3))) {
				ridePath = new Vector3[partialPathLength + 1];
				ridePath[0] = (Vector3)startVector;
				startIndex = 1;
			} else {
				ridePath = new Vector3[partialPathLength];
			}
			// Assemble path
			for (int i = startIndex; i < partialPathLength + startIndex; i++) {
				ridePath[i] = path.pathVectors[start + i - startIndex];
			}
		} 
		// ...Otherwise, just ride the provided path from the beginning
		else {
			ridePath = path.pathVectors;
		}

		// Ride the path!
		Debug.Log("riding path!");
		for (int i = 0; i < ridePath.Length - 1; i++) {
			Debug.DrawLine(ridePath[i], ridePath[i+1], Color.red, 5.0f);
		}

		LeanTween.moveSpline(gameObject, ridePath, 10f).setSpeed(onPathSpeed).setOnComplete(LeavePath);
		onPath = true;
		currentPath = path;
		currentPathVectors = ridePath;
	}

	void LeavePath() {
		// Set velocity according to last direction of travel on path
		int last = currentPathVectors.Length - 1;
		Vector2 direction = (currentPathVectors[last] - currentPathVectors[last - 1]).normalized;
		velocity = direction * offPathSpeed;

		// Exit from end of path
		Debug.Log("exiting path!");
		LeanTween.cancel(gameObject);
		onPath = false;
		currentPath = null;
		currentPathVectors = null;
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

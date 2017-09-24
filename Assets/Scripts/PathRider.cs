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
	LTSpline currentSpline = null;
	Vector3 currentPathVector = Vector3.zero;
	int currentPathVectorIndex = 0;
	Vector3 nextPathVector = Vector3.zero;

	float startTime = 1f;
	float startTimer = 0f;
	bool started = false;

	bool onPath;
	Vector2 velocity;
	Collider2D cd;
	Rect box;

	// Raycasting
	int numRays = 5;
	int numPathRays = 1;
	RaycastHit2D closestHorzHitInfo;
	RaycastHit2D closestVertHitInfo;
	RaycastHit2D closestPathHitInfo;

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
			// Calculate velocity based on movement along path
			if (currentSpline.ratioAtPoint(nextPathVector) < currentSpline.ratioAtPoint(transform.position)) {
				currentPathVector = nextPathVector;
				currentPathVectorIndex++;
				nextPathVector = currentPathVectors[currentPathVectorIndex + 1];
			}
			velocity = (nextPathVector - currentPathVector).normalized * onPathSpeed;
		}

		// Check for non-path objects
		// Determine first and last horizontal rays
		Vector2 minHorzRay = new Vector2(box.center.x + (box.size.x / 2) * ((velocity.x > 0) ? 1 : -1), box.yMin);
		Vector2 maxHorzRay = new Vector2(box.center.x + (box.size.x / 2) * ((velocity.x > 0) ? 1 : -1), box.yMax);

		// Calculate ray distance (current fall speed) and direction
		//float rayDistance = box.height / 2 + Mathf.Abs(velocity.y * Time.deltaTime);
		// TODO: This can't be right. The 1.5f is just to lengthen it a little to reduce the chance of missing an edge collider.
		float rayDistance = Mathf.Abs(velocity.magnitude * 1.5f * Time.deltaTime);

		// Cast horizontal rays
		RaycastHit2D[] horzHitInfo = new RaycastHit2D[numRays];
		bool horzHit = false;
		float closestHorzHit = float.MaxValue;
		int closestHorzHitIndex = 0;
		for (int i = 0; i < numRays; i++) {
			// Create and cast rays
			float lerpDistance = (float)i / (float)(numRays - 1);
			Vector2 rayOrigin = Vector2.Lerp(minHorzRay, maxHorzRay, lerpDistance);
			Ray2D ray = new Ray2D(rayOrigin, velocity.normalized);
			horzHitInfo[i] = Physics2D.Raycast(rayOrigin, velocity.normalized, rayDistance, 1 << LayerMask.NameToLayer("Consumable"));
			Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.magenta, 1f);
			// Check raycast results and keep track of closest hit
			if (horzHitInfo[i].fraction > 0) {
				horzHit = true;
				if (horzHitInfo[i].fraction < closestHorzHit) {
					closestHorzHit = horzHitInfo[i].fraction;
					closestHorzHitIndex = i;
					closestHorzHitInfo = horzHitInfo[i];
				}
			}
		}

		// Check and handle what we hit
		if (horzHit) {
			Consumable consumable = closestHorzHitInfo.collider.gameObject.GetComponent<Consumable>();
			if (consumable) {
				consumable.gameObject.SetActive(false);
			}
		}

		// Determine first and last vertical rays
		Vector2 minVertRay = new Vector2(box.xMin, box.center.y + (box.size.y / 2) * ((velocity.y > 0) ? 1 : -1));
		Vector2 maxVertRay = new Vector2(box.xMax, box.center.y + (box.size.y / 2) * ((velocity.y > 0) ? 1 : -1));

		// Cast vertical rays
		RaycastHit2D[] vertHitInfo = new RaycastHit2D[numRays];
		bool vertHit = false;
		float closestVertHit = float.MaxValue;
		int closestVertHitIndex = 0;
		for (int i = 0; i < numRays; i++) {
			// Create and cast rays
			float lerpDistance = (float)i / (float)(numRays - 1);
			Vector2 rayOrigin = Vector2.Lerp(minVertRay, maxVertRay, lerpDistance);
			Ray2D ray = new Ray2D(rayOrigin, velocity.normalized);
			vertHitInfo[i] = Physics2D.Raycast(rayOrigin, velocity.normalized, rayDistance, 1 << LayerMask.NameToLayer("Consumable"));
			Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.magenta, 1f);
			// Check raycast results and keep track of closest hit
			if (vertHitInfo[i].fraction > 0) {
				vertHit = true;
				if (vertHitInfo[i].fraction < closestVertHit) {
					closestVertHit = vertHitInfo[i].fraction;
					closestVertHitIndex = i;
					closestVertHitInfo = vertHitInfo[i];
				}
			}
		}

		// Check and handle what we hit
		if (vertHit) {
			Consumable consumable = closestVertHitInfo.collider.gameObject.GetComponent<Consumable>();
			if (consumable) {
				consumable.gameObject.SetActive(false);
			}
		}

		// Check for paths
		// Cast the ray to check for paths
		RaycastHit2D[] pathHitInfo = new RaycastHit2D[numPathRays];
		bool pathHit = false;
		float closestPathHit = float.MaxValue;
		int closestPathHitIndex = 0;
		for (int i = 0; i < numPathRays; i++) {
			// Create and cast ray
			//float lerpDistance = (float)i / (float)((numPathRays > 1) ? numPathRays - 1 : numPathRays);
			Vector2 rayOrigin = transform.position; //Vector2.Lerp(minRay, maxRay, lerpDistance);
			Ray2D ray = new Ray2D(rayOrigin, velocity.normalized);
			pathHitInfo[i] = Physics2D.Raycast(rayOrigin, velocity.normalized, rayDistance, 1 << LayerMask.NameToLayer("Path"));
			Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan, 1f);
			// Check raycast results and keep track of closest path hit
			if (pathHitInfo[i].fraction > 0) {
				pathHit = true;
				if (pathHitInfo[i].fraction < closestPathHit) {
					closestPathHit = pathHitInfo[i].fraction;
					closestPathHitIndex = i;
					closestPathHitInfo = pathHitInfo[i];
				}
			}
		}

		// Check and handle what path we hit
		if (pathHit) {
			RidePath path = closestPathHitInfo.collider.gameObject.GetComponent<RidePath>();
			// If path exists and is drawOrder is greater than current path's, snap to hit point
			if (path)
				if (currentPath == null || (path.drawOrder > currentPath.drawOrder && path != currentPath)) {
					PlaceSelfInPath(closestPathHitInfo.point, path);
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
				if (currentPath == null)
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

	void RidePath(RidePath ridePath, int start = 0, Vector3? startVector = null) {
		Vector3[] path = new Vector3[0];

		// Trim beginning of vector array if starting index is not 0...
		if (start != 0) {
			int partialPathLength = ridePath.pathVectors.Length - start;
			int startIndex = 0;
			// Change starting vector if provided
			if (startVector != null && startVector.GetType().Equals(typeof(Vector3))) {
				path = new Vector3[partialPathLength + 1];
				path[0] = (Vector3)startVector;
				startIndex = 1;
			} else {
				path = new Vector3[partialPathLength];
			}
			// Assemble path
			for (int i = startIndex; i < partialPathLength + startIndex; i++) {
				path[i] = ridePath.pathVectors[start + i - startIndex];
			}
		} 
		// ...Otherwise, just ride the provided path from the beginning
		else {
			path = ridePath.pathVectors;
		}

		// Ride the path!
		Debug.Log("riding path!");
		for (int i = 0; i < path.Length - 1; i++) {
			Debug.DrawLine(path[i], path[i+1], Color.red, 5.0f);
		}

		onPath = true;
		currentPath = ridePath;
		currentPathVectors = path;
		currentSpline = CreateSpline(path);
		currentPathVectorIndex = 0;
		currentPathVector = path[currentPathVectorIndex];
		nextPathVector = path[currentPathVectorIndex + 1];
		LeanTween.moveSpline(gameObject, currentSpline.pts, 10f).setSpeed(onPathSpeed).setOnComplete(LeavePath);
	}

	void LeavePath() {
		Debug.Log("trying to leave");
		if (Mathf.Abs(currentSpline.ratioAtPoint(transform.position) - 1.0f) < 0.001f ) {
			Debug.Log("approved to leave");
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
			currentSpline = null;
		} else {
			LeanTween.moveSpline(gameObject, CreateSpline(currentPathVectors), 10f).setSpeed(onPathSpeed).setOnComplete(LeavePath);
		}
	}

	LTSpline CreateSpline(Vector3[] path) {
		// The first and last vectors in a spline indicate the angle of the points.
		// We don't care about the angle, so we just need to duplicate the first and last items.
		Vector3[] splinePath = new Vector3[path.Length + 2];
		splinePath[0] = path[0];
		splinePath[splinePath.Length - 1] = path[path.Length - 1];
		for (int i = 0; i < path.Length; i++) {
			splinePath[i + 1] = path[i];
		}

		return new LTSpline(splinePath);
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

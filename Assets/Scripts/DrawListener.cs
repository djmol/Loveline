using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class DrawListener : MonoBehaviour {

	public float startWidth = 1.0f;
	public float endWidth = 1.0f;
	public float threshold = 0.001f;
	public GameObject playerPaths;
	public Material drawingPathMaterial;
	public Material pathMaterial;
	public Slider drawResourceSlider;
	public float drawResource = 10f;
	public float maxDrawResource = 10f;
	public float drawResourceRegenRate = .01f;

	List<Vector3> pathPoints = new List<Vector3>();
	int pathCount = 0;
	Vector3 lastPos = Vector3.one * float.MaxValue;
	int pathsDrawn = 0;
	bool drawnToDepletion = false;

	LineRenderer lineRenderer;
	Camera thisCamera;

	// Use this for initialization
	void Start () {
		thisCamera = Camera.main;
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.sortingLayerName = "Character";

		// Resources
		drawResourceSlider.minValue = 0f;
		drawResourceSlider.maxValue = maxDrawResource;
		drawResourceSlider.value = maxDrawResource;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input
		bool mouseDown = Input.GetMouseButton(0);
		bool mouseUp = Input.GetMouseButtonUp(0);

		// Regenerate resources
		drawResource = Mathf.Min(drawResource + (drawResourceRegenRate * Time.deltaTime), maxDrawResource);

		// Draw path while dragging
		if (mouseDown) {
			Vector3 mousePos = Input.mousePosition;
			Vector3 mouseWorld = thisCamera.ScreenToWorldPoint(mousePos);
			mouseWorld.z = 0;

			float dist = Vector3.Distance(lastPos, mouseWorld);
			if (dist > threshold && drawResource > 0f && !drawnToDepletion) {
				// Use up resources			
				if (Mathf.Abs(dist) != Mathf.Infinity) {
					drawResource = Mathf.Max(drawResource - dist, 0f);
					if (drawResource == 0) 
						drawnToDepletion = true;
				}

				lastPos = mouseWorld;
				if (pathPoints == null)
					pathPoints = new List<Vector3>();
				pathPoints.Add(mouseWorld);

				UpdatePath();
			}
		} else if (mouseUp) {
			CompletePath();
			ResetPath();
			drawnToDepletion = false;
		}

		// Update UI
		drawResourceSlider.value = drawResource;
	}

     void UpdatePath() {
		lineRenderer.startWidth = startWidth;
		lineRenderer.endWidth = endWidth;
		lineRenderer.positionCount = pathPoints.Count;
		lineRenderer.material = drawingPathMaterial;

		for(int i = pathCount; i < pathPoints.Count; i++) {
			lineRenderer.SetPosition(i, pathPoints[i]);
		}

		pathCount = pathPoints.Count;
     }

	 void CompletePath() {
		// Create new path
		pathsDrawn++;
		GameObject pathGO = new GameObject("Path " + pathsDrawn);
		pathGO.transform.parent = playerPaths.transform;
		pathGO.layer = LayerMask.NameToLayer("Path");
		LineRenderer lineRend = pathGO.AddComponent<LineRenderer>();
		lineRend.material = pathMaterial;
		lineRend.startWidth = startWidth;
		lineRend.endWidth = endWidth;
		lineRend.positionCount = pathPoints.Count;
		lineRend.SetPositions(pathPoints.ToArray());
		RidePath ridePath = pathGO.AddComponent<RidePath>();
		ridePath.drawOrder = pathsDrawn;
	 }

	 void ResetPath() {
		lastPos = Vector3.one * float.MaxValue;
		pathPoints.Clear();
		pathCount = 0;
		lineRenderer.positionCount = 0;
	 }
}

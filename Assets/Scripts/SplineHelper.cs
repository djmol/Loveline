using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SplineHelper {
	public static LTSpline CreateSpline(Vector3[] path) {
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
}

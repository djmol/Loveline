using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpawner : MonoBehaviour {

	public Vector3 minimumScale;
	
	SpriteRenderer rend;
	LevelManager lm;

	// Use this for initialization
	void Start () {
		lm = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
		rend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (lm.addLifePoints <= 0) {
			rend.enabled = false;
		} else {
			rend.enabled = true;
			rend.gameObject.transform.localScale = minimumScale + (minimumScale * (lm.addLifePoints / lm.maxAddLifePoints));
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz;

public class AddSpawner : MonoBehaviour {

	public Vector3 minimumScale;
	public GameObject addPrefab;
	Vector3 maximumScale;
	SpriteRenderer sprRend;
	LevelManager lm;

	// Use this for initialization
	void Start () {
		lm = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
		sprRend = GetComponent<SpriteRenderer>();
		maximumScale = addPrefab.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (lm.addLifePoints <= 0) {
			sprRend.enabled = false;
		} else if (lm.addLifePoints < lm.maxAddLifePoints) {
			sprRend.enabled = true;
			sprRend.color = new Color(1f, 1f, 1f, (lm.addLifePoints / lm.maxAddLifePoints));
			sprRend.gameObject.transform.localScale = minimumScale + ((maximumScale - minimumScale) * (lm.addLifePoints / lm.maxAddLifePoints));
		} else {
			lm.addLifePoints -= lm.maxAddLifePoints;
			Instantiate(addPrefab, transform.position, Quaternion.identity);
		}
	}
}

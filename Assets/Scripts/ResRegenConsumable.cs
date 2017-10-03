using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResRegenConsumable : Consumable {

	public float restorePercentResource;
	public override System.Type type {
		get {
			return typeof(ResRegenConsumable);
		}
		set { }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

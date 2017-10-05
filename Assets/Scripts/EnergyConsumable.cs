using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumable : Consumable {

	public float addLifePoints = 1f;
	public AudioSource audioSource;
	public override System.Type type {
		get {
			return typeof(EnergyConsumable);
		}
		set { }
	}

	// Use this for initialization
	void Start () {
		audioSource = GetComponentInParent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConsumable : Consumable {

	public int addLifePoints = 1;
	public EnergyConsumableManager manager { get; private set; }
	public override System.Type type {
		get {
			return typeof(EnergyConsumable);
		}
		set { }
	}

	// Use this for initialization
	void Start () {
		manager = GetComponentInParent<EnergyConsumableManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

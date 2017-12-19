using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour {

	public List<GameObject> pickups = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Carryable") {
			pickups.Add (other.gameObject);
		}
	}

	void OnTriggerExit(Collider other){
		pickups.Remove (other.gameObject);
	}
}

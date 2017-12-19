using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other){

		//TODO: change this behavior to subtract health from enemies or whatever

		if (other.attachedRigidbody) {
			other.attachedRigidbody.AddForce (Vector3.up * 200);
		}
	}
}

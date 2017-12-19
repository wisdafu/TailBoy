using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveWarpZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
      if (collider.gameObject.tag == "Player")
      {
        Application.LoadLevel("CaveScene");
      }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleCollider : MonoBehaviour {


    public GameObject toggle_object;
    public bool initially_invisible;
    public bool keep_active;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = toggle_object.GetComponent<Animator>();
        if (initially_invisible) {
          toggle_object.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
		if (col.tag == "Carryable")
        {
      if (initially_invisible)
        toggle_object.SetActive(true);
      else
				toggle_object.SetActive (false);
			Debug.Log ("Collided with: " + col.name);
		}
        
    }

    void OnTriggerStay(Collider col)
    {

    }

    void OnTriggerExit(Collider col)
    {
		if (col.tag == "Carryable") {
			if (initially_invisible) {
				if (!keep_active)
					toggle_object.SetActive (false);
			} else {
				toggle_object.SetActive (true);
			}
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

  GameObject enemy;
  Rigidbody rb;

	// Use this for initialization
	void Start () {
      enemy = GetComponent<GameObject>();
      rb = GetComponent<Rigidbody>();

  }

  // Update is called once per frame
  void Update () {
      
      
	}

  void OnTriggerEnter(Collider collision)
  {
    if (collision.gameObject.tag == "Player")
    {
      //Enemy hit player, damage player
    }else if(rb.tag == "attack")
    {
      //Enemy was hit by attack
			Destroy(this.gameObject);
    }
  }
}

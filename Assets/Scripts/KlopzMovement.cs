using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KlopzMovement : MonoBehaviour {

	public GameObject[] waypoints;
	public float followDistance = 20f;

	Transform player;
	NavMeshAgent nav;

	GameObject enemy;
	Rigidbody rb;
    ParticleSystem ps;
    private bool dead;

	private int currentWaypoint = 0;
    
	// Use this for initialization
	void Start () {
    	nav = GetComponent<NavMeshAgent> ();
    	player = GameObject.FindGameObjectWithTag("Player").transform;
		enemy = GetComponent<GameObject>();
		rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        ps.enableEmission = false;
        dead = false;

		waypoints = GameObject.FindGameObjectsWithTag ("KlopsWP");
	}
	
	// Update is called once per frame
	void Update () {
      if (player && !dead)
      {
        //if player is close, follow player
        float playerDist = Vector3.Distance(player.position, transform.position);

        if (playerDist < followDistance)
        {
          nav.SetDestination(player.position);
        }
        else
        {
          if (waypoints.Length > 0)
            Patrol();
          //Flock ();
        }
      }
	}

	void OnTriggerEnter(Collider collision)
  {
    if (collision.gameObject.tag == "Player")
		{
      //Enemy hit player, damage player
      //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().applyHit();
		}else if(collision.gameObject.tag == "attack")
		{
            //Enemy was hit by attack
            dead = true;
            this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            this.rb.AddForce(Vector3.up*600);
			Destroy(this.gameObject,2);
            this.ps.enableEmission = true;

		}
	}

	void Flock(){
		GameObject[] klopzes = GameObject.FindGameObjectsWithTag ("klopz");
		Vector3 pos = transform.position;
		float curDistance;
		foreach (GameObject k in klopzes) {
			Vector3 diff = k.transform.position - pos;
			float dist = diff.sqrMagnitude;
			if (dist < followDistance) {
				nav.SetDestination (k.transform.position);
			}
		}
	}

	void Patrol(){
		
		Transform currentTarget = waypoints[currentWaypoint].transform;
		float dist = Vector3.Distance (currentTarget.position, transform.position);

		nav.SetDestination (currentTarget.position);
		//print ("Heading to " + currentTarget.position);

		if (dist <= 5f) {
			currentWaypoint++;
			if (currentWaypoint > waypoints.Length-1)
				currentWaypoint = 0;
			//print ("Current waypoint: " + currentWaypoint);
		}
			
	}
}

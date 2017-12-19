using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6.0f;
    public float run_speed = 50.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float glide_gravity = 10.0f;
    public float mouseSensitivity = 100.0f;
    public float pushStrength = 4.0f;
    public GameObject attackSphere;
	public GameObject pickupTrigger;
    public UnityEngine.UI.Text overlay;
    public int score;

    private CharacterController controller;
    private Transform cam;
    private Vector3 moveDirection = Vector3.zero;
    private float rotX;
    private float rotY;
	private float prevYDirection = 0;
	private float jumpTimer = 0f;
    private bool attacking = false;
    private bool carrying = false;
    private GameObject carryingObject;
    private bool gliding = false;
	private Pickups availablePickups;
    private bool help_collapsed = true;
    private AudioSource[] audioSrcs;
    private bool falling = false;
    private bool isDamaged;
    private bool canDamage;
    private int invulnTimer;
    private int regenTimer;
    public UnityEngine.UI.Text healthUI;
    private AudioSource audioSrcHit;
    private AudioSource audioSrcJump;
    private AudioSource audioDamage;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        cam = transform.Find("Main Camera");

        attackSphere.SetActive(false);

		//get Pickups object
		availablePickups = pickupTrigger.GetComponent<Pickups>();

        //initialize rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        Vector3 camRot = cam.localRotation.eulerAngles;
        rotX = camRot.x;

        score = 0;
        audioSrcs = GetComponents<AudioSource>();
        audioSrcHit = audioSrcs[0];
        audioSrcJump = audioSrcs[1];
        audioDamage = audioSrcs[2];

		audioSrcJump.playOnAwake = false;

        isDamaged = false;
        canDamage = true;
        invulnTimer = 0;
        regenTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        if (Input.GetKey(KeyCode.LeftShift))
            moveDirection = new Vector3 (Input.GetAxis ("Horizontal")*3, prevYDirection, Input.GetAxis ("Vertical")*3);
        else
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), prevYDirection, Input.GetAxis("Vertical"));
        //make direction vector relative to world space
        moveDirection = transform.TransformDirection (moveDirection);

        //print(gliding);

        if(controller.isGrounded && gliding)
        {
            gliding = false;
            become_normal_model();
            //print("Turning gliding off");
        }

		//jumping
		if (controller.isGrounded) {
			//only jump if grounded and falling
			if (Input.GetButton ("Jump") && !falling) {
				jumpTimer = 0;
				moveDirection.y = 2;
                audioSrcJump.PlayOneShot(audioSrcJump.clip);
            } 
			//reset move direction if we've just landed
			if (moveDirection.y <= 0) {
				moveDirection.y = 0;
			}
			//prevent "bouncing" by only resetting falling flag when jump is released
			if(!Input.GetButton("Jump"))
				falling = false;
		}
		//falling / gliding
        if (!controller.isGrounded && !gliding && moveDirection.y <= 0 && Input.GetKey (KeyCode.E)) {
			gliding = true;
			become_flying_model ();
			moveDirection.y -= glide_gravity * Time.deltaTime;
			//print("Pressed Glide");
		} else if (gliding) {
			//print("Down at gliding speed");
			moveDirection.y -= glide_gravity * Time.deltaTime;
		} else if (Input.GetButton ("Jump") && jumpTimer < 5 && !falling) {
			moveDirection.y += gravity * Time.deltaTime;
			jumpTimer ++;
		} else if (Input.GetButton ("Jump") && jumpTimer >= 5 && !falling) {
			jumpTimer = 0;
			falling = true;
			moveDirection.y -= gravity * Time.deltaTime;
		}
        else
        {
            //print("Down at normal speed");
            moveDirection.y -= gravity * Time.deltaTime;
        }
		prevYDirection = moveDirection.y;

		//print (controller.isGrounded ? "GROUNDED" : "NOT GROUNDED");
		//print (moveDirection.y);
        controller.Move(moveDirection * Time.deltaTime * speed);


        //mouse look
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        Quaternion boyRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        transform.rotation = boyRotation;

        Quaternion camRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        cam.rotation = camRotation;

        //attack
        if (Input.GetButton("Fire1"))
        {
            if (attacking)
            {
                attackSphere.SetActive(false);
                //attacking = false;
            }
            else
            {
                audioSrcHit.PlayOneShot(audioSrcHit.clip);
                attackSphere.SetActive(true);
                attacking = true;
      }

        }
        else
        {
            attackSphere.SetActive(false);
            attacking = false;
        }

		//pick up
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (carrying)
            {
                dropObject();
            }
            else
            {
                pickupObject();
            }

        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (help_collapsed)
            {
                overlay.text = "WASD - Movement\nSpace - Jump\nE - Glide\nF - Pick Items\nShift - Sprint\nH - Close Menu";
                help_collapsed = false;
            }
            else
            {
                overlay.text = "H - Controls";
                help_collapsed = true;

            }
            
        }
    }

    void become_flying_model()
    {
        
    }

    void become_normal_model()
    {

    }

    //since the character controller has no rigidbody, we need to handle collisions
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 force;
    GameObject hitObject = hit.gameObject;
    if (hitObject.tag == "Enemy" && canDamage)
    {
      applyHit();
    }
        

        //check that object has a rigidbody
        if (body == null || body.isKinematic)
            return;

        //if we landed on top of the object, do nothing
        if (hit.moveDirection.y < 0.0f)
        {
            return;
        }
        else
        {
            force = hit.controller.velocity * pushStrength;
        }

        body.AddForceAtPosition(force, hit.point);
    }

  void applyHit()
  {
    //Debug.Log("Damage: " + isDamaged+" | CanDamage: "+canDamage);
    if (isDamaged)  //Check if damaged
    {
      //dies
      Application.LoadLevel(Application.loadedLevel);
    }
    else
    {
      //apply damage
      audioSrcJump.PlayOneShot(audioDamage.clip);
      isDamaged = true;
      canDamage = false;
      healthUI.text = "Ouch!! Hurry get back!!";
      InvokeRepeating("Invuln", 0.0f, 0.125f);
    }
  }

  void Invuln()
  {
    invulnTimer++;

    if(invulnTimer > 16)  //2 Seconds * 8 calls per second
    {
      canDamage = true;
      invulnTimer = 0;
      CancelInvoke("Invuln");
      healthUI.text = "Hold off until Tail Boy can regenerate.";
      InvokeRepeating("Regen",0.0f,0.25f);
    }
  }

  void Regen()
  {
    regenTimer++;

    if (regenTimer > 28)  //7 Seconds * 4 calls per second
    {
      regenTimer = 0;
      healthUI.text = "Tail Boy is good to go!";
      CancelInvoke("Regen");
    }
  }

    void dropObject()
    {
        if (carrying)
        {

            Debug.Log("Dropping " + carryingObject.ToString());

            carryingObject.transform.SetParent(null);
            carryingObject.GetComponent<Rigidbody>().useGravity = true;
            carryingObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            carrying = false;
        }
    }

    void pickupObject()
    {
		/*
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.tag == "Carryable")
            {
                Debug.Log("Pickup " + hit.collider.name);

                carryingObject = hit;
                carryingObject.transform.SetParent(this.gameObject.transform);
                carryingObject.rigidbody.useGravity = false;
                carryingObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                carryingObject.transform.localRotation = this.gameObject.transform.rotation;
                carryingObject.transform.position = this.gameObject.transform.position;
                carryingObject.transform.localPosition = new Vector3(0,0,2);

                carrying = true;
            }
        }
        */

		//if no pick-upable objects, return
		if (availablePickups.pickups.Count == 0)
			return;

		if (availablePickups.pickups.Count == 1) {
			//if only one object in range, grab it
			carryingObject = availablePickups.pickups [0];

			grab (carryingObject);

			carrying = true;
		} /*else {
			//more than one object in range, grab the closest one
			float minDist = 20; //outside the range of the pickup collider
			foreach (GameObject p in availablePickups.pickups) {
				float pDist = Vector3.Distance (transform.position, p.transform.position);
				if (pDist < minDist) {
					
				}
			}
		}*/
    }

	void grab(GameObject o){
		o.transform.SetParent (this.gameObject.transform);

		//get pickup's rigidbody and freeze it
		Rigidbody rb = o.GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeAll;

		//put it in the right place
		o.transform.localRotation = this.gameObject.transform.rotation;
		o.transform.position = this.gameObject.transform.position;
		o.transform.localPosition = new Vector3 (0, 0, 2);
	}

    public void ScoreAdd()
    {
      score++;
      GameObject.FindGameObjectWithTag("ScoreUI").GetComponent<ScoreUI>().updateText(score);
    }
}

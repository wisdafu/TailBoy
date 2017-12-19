using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMo : MonoBehaviour
{

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float mouseSensitivity = 100.0f;
    public float pushStrength = 4.0f;
    public GameObject attackSphere;

    private CharacterController controller;
    private Transform cam;
    private Vector3 moveDirection = Vector3.zero;
    private float rotX;
    private float rotY;
    private bool attacking = false;
    private bool carrying = false;
    private RaycastHit carryingObject;
    private GameObject tail;
    private GameObject tail_hinge;
    private Vector3 tailPosition;
    private Vector3 tail_hinge_location;
    private bool tethered;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = transform.Find("Main Camera");

        attackSphere.SetActive(false);

        //initialize rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        Vector3 camRot = cam.localRotation.eulerAngles;
        rotX = camRot.x;

        tail = GameObject.Find("Tail End");
        tail_hinge = GameObject.Find("TailHinge");
        tethered = true;
    }

    // Update is called once per frame
    void Update()
    {

        tailPosition = tail.transform.position;
        tail_hinge_location = tail_hinge.transform.position;

        //movement
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        if (tethered && Vector3.Distance(tailPosition, tail_hinge_location) > 12)
        {
            Debug.Log("at tail length");
            Transform tail_trans = tail.transform;
            transform.position = Vector3.MoveTowards(transform.position, tail_trans.position, 10);
        }

        //mouse look
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        Quaternion boyRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        transform.rotation = boyRotation;

        Quaternion camRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        cam.rotation = camRotation;

        //fall
        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime * speed);

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
                attackSphere.SetActive(true);
                attacking = true;
            }

        }
        else
        {
            attackSphere.SetActive(false);
            attacking = false;
        }

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
    }

    //since the character controller has no rigidbody, we need to handle collisions
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 force;

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

    void dropObject()
    {
        if (carrying)
        {

            Debug.Log("Dropping " + carryingObject.ToString());

            carryingObject.transform.SetParent(null);
            carryingObject.rigidbody.useGravity = true;

            carrying = false;
        }
    }

    void pickupObject()
    {
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
                carryingObject.transform.localRotation = this.gameObject.transform.rotation;
                carryingObject.transform.position = this.gameObject.transform.position;

                carrying = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;


public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject winTextObject;


    public float speed;
    public float jump_strength;

    private Rigidbody rb;
    
    private float movementX;
    private float movementY;

    private int pickup_count; // How many objects have we picked up so far?
    private const int NUM_PICKUP = 16; // How many objects total?
    private bool on_ground; // Are we touching the ground currently?

    private const int IN_AIR_JUMPS = 1; // How many times can we jump in the air?
    private int jump_ctr; // Tracks the number of jumps in 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickup_count = 0;
        SetCountText ();

        // Set the text property of the Win Text UI to an empty string, making the 'You Win' (game over message) blank
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump()
    {
        print("jump");
        Vector3 jump = new Vector3(0.0f, jump_strength, 0.0f);
        if (on_ground) {
            rb.AddForce(jump);
        } else if (jump_ctr < IN_AIR_JUMPS) {
            jump_ctr++;
            rb.AddForce(jump);
        }
        
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
        
        // FixedUpdate is called before OnCollisionStay, both of which happen
        // before the functions taking input. 
        // source: https://docs.unity3d.com/Manual/ExecutionOrder.html
        // This means that we can mark on_ground to false here, it may or
        // may not get changed to true when we check collisions, and then
        // determine if the jump is valid based on that up-to-date info. 
        on_ground = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp")) {
            other.gameObject.SetActive(false);
            pickup_count++;
            SetCountText();
        }
        
    }

    void SetCountText()
    {
        countText.text = "Count: " + pickup_count.ToString();

        if (pickup_count >= NUM_PICKUP) 
        {
                    // Set the text value of your 'winText'
                    winTextObject.SetActive(true);
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.collider.gameObject.CompareTag("Ground")) {
            on_ground = true;
            jump_ctr = 0;
        }
    }
}

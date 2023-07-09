using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent customEvent;

    //Movement variables
    public float speed = 10.0f;
    [Range(0f, 1.0f)]
    public float accelerationFactor = 0f;
    [Range(0f, 1.0f)]
    public float decelerationFactor = 1f;

    private float horizontalInput = 0f;
    private Vector2 newVelocity = new Vector2(0f, 0f);

    //Jump variables
    public int jumpCount = 0;
    public float jumpForce = 0f;
    public float fallAcceleration = 0f;
    public float defaultGravity = 10f; 

    private float verticalInput = 0f;

    //Death bool
    public bool death = false;

    //Animator bool
    public Animator animator;
    private bool facingRight = false;
    private SpriteRenderer spriteRenderer;

    //Game object attached to character with trigger for checking ground
    public GameObject groundCheckerObj;
    private GroundChecker groundChecker;

    //Game Components
    private Rigidbody2D rBody;
    
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        rBody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rBody.gravityScale = defaultGravity;
        groundChecker = groundCheckerObj.GetComponent<GroundChecker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Animator Jump
        animator.SetBool("IsJumping", true);

        // ****************************************************************
        // --------------------------Jump Movement-------------------------
        // ****************************************************************

        //If player let go of space button mid jump
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!groundChecker.touchingGround)
            {
                //Fall faster if space is let go and not touching the ground
                rBody.gravityScale = fallAcceleration * defaultGravity;
            }
        }

        //If player press the space button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (groundChecker.ableToJump && !death)
            {
                rBody.gravityScale = defaultGravity;

                //Jump
                rBody.velocity = new Vector2(rBody.velocity.x, 0f);
                rBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                groundChecker.curJump -= 1;
            }
        }

        if (groundChecker.touchingGround)
        {
            //If touching ground, set gravity to normal
            rBody.gravityScale = defaultGravity;

            animator.SetBool("IsJumping", false);
        }
    }

    // Update for Physics
    private void FixedUpdate()
    {
        //Set new velocity to rigidbody velocity (initialization)
        newVelocity.x = rBody.velocity.x;
        newVelocity.y = rBody.velocity.y;

        // ****************************************************************
        // ---------------------Horizontal Movement------------------------
        // ****************************************************************

        //Right movement
        if (horizontalInput == 1)
        {
            //Increase x velocity by the acceleration factor
            newVelocity.x += speed * accelerationFactor;

            //Flip if not facing right
            if (!facingRight && !death)
            {
                spriteRenderer.flipX = true;
                facingRight = true;
            }

            //If updated speed exceed speed limit, set it to the max speed
            if (newVelocity.x > speed)
            {
                newVelocity.x = speed;
            }
        }
        else if (horizontalInput == -1) //Left movement
        {
            //Decrease x velocity by the acceleration factor
            newVelocity.x -= speed * accelerationFactor;

            //Flip if not facing left
            if (facingRight && !death)
            {
                spriteRenderer.flipX = false;
                facingRight = false;
            }

            //If updated speed exceed speed limit, set it to the max speed
            if (newVelocity.x < -speed)
            {
                newVelocity.x = -speed;
            }
        }
        else if (horizontalInput == 0) //No horizontal movement button pressed
        {
            float decelerationSpeed = speed * decelerationFactor;
            //Debug.Log("current deceleration speed: " + decelerationSpeed);

            //Animator Doesn't Move
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));

            //If speed is within this threshold, set speed to 0
            if (-decelerationSpeed <= newVelocity.x && newVelocity.x <= decelerationSpeed)
            {
                //Set x velocity to 0
                newVelocity.x = 0; //Should ignore all the other case because speed is 0
            }

            //Negative velocity case
            if (newVelocity.x < 0)
            {
                newVelocity.x += decelerationSpeed;
            }
            else if (newVelocity.x > 0) //Positive velocity case
            {
                newVelocity.x -= decelerationSpeed;
            }

            //Debug.Log("updated x velocity: " + newVelocity.x);
        }

        // ****************************************************************
        // ------------------------Crouch Movement-------------------------
        // ****************************************************************

        if (verticalInput == -1 && groundChecker.touchingGround)
        {
            //Crouch
            //Update horizontal movement for crouching
        }

        // ****************************************************************
        // ------------------------Velocity Update-------------------------
        // ****************************************************************

        Debug.Log("Death value is " + death);

        //Update velocity with the updated velocity
        if (!death)
        {
            //Animator go left
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));

            rBody.velocity = newVelocity;
        }
    }


}

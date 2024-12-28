using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentScript : MonoBehaviour
{
    //Refrences
    public Animator anim;
    public CharacterController controller;
    public Transform cam;
    public Rigidbody rb;
    public Transform groundCheck;

    //Variables
    //Run Variables
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Vector3 velocity;
    public bool running = false;
    bool allowMove = true;
    //Jump Variables
    public float jumpSpeed = 10f;
    public float gravity = -9.8f;
    public float groundDistance = 0.4f;
    bool isGrounded;
    public bool inAir;
    public LayerMask groundMask;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    public float landTimer;
    private float landTimerReset = -0.1f;
    public float justTouchedGround;
    private float justTouchedGroundRest = -0.5f;
    private bool isjumping = false;
    //Power Jump Variables
    public bool canHoldQ;
    public float powerJumpTimer;
    private float defaultPowerJumpTimer = 1.25f;
    public float powerJumpPower = 5;
    bool powerJumpAvailable = false;
    bool powerJumped = false;
    bool justPowerJumped = false;
    public float settingPowerJumpFalse;
    public float settingPowerJumpFalseReset = 0.001f;
    //Dash Down Variables
    public float dashDownGravity = 2f;
    public bool dashDown = false;


    // Start is called before the first frame update
    void Start()
    {
      rb = GetComponent<Rigidbody>();
      anim = GetComponent<Animator>();
      powerJumpTimerRest();
      justPowerJumpedTimerRest();
      settingPowerJumpFalsee();
    }

    // Update is called once per frame
    void Update()
    {
      //Checks if Grounded or not
      isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
      if(isGrounded && velocity.y <0)
      {
        coyoteTimeCounter = coyoteTime;
        //velocity.y = -2f;
      }
      else
      {
        coyoteTimeCounter -= Time.deltaTime;
      }

      //Checks if in Air
      if(isGrounded == false)
      {
        inAir = true;
        anim.SetBool("inAirr", true);
        anim.SetBool("jump", true);
        anim.SetBool("isIdle", false);
        canHoldQ = false;
      }
      else
      {
        inAir = false;
        isjumping = false;
        anim.SetBool("inAirr", false);
        anim.SetBool("jump", false);
        ////////anim.SetBool("landed", true);
        if(justPowerJumped)
        {
          anim.SetBool("powerJump", true);
        }
        else
        {
          anim.SetBool("landed", true);
        }

        if(dashDown)
        {
          anim.SetBool("powerJump", true);
        }
        else
        {
          anim.SetBool("landed", true);
        }
        
        landTimer += Time.deltaTime;
        if(dashDown)
        {
          settingPowerJumpFalse += Time.deltaTime;
        }

        if(justPowerJumped)
        {
          settingPowerJumpFalse += Time.deltaTime;
        }
        
        if (landTimer > 0.0001)
        {
          anim.SetBool("landed", false);
        }

        if(settingPowerJumpFalse > 0.01)
        {
          anim.SetBool("powerJump", false);
        }

        if(running == false)
        {
          canHoldQ = true;
        }
        else
        {
          canHoldQ = false;
        }
      }

      //Movment (Has option to be disabled for cases such as powering jump)
      if(allowMove == true)
      {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f) 
        {
          running = true;
          float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
          float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
          transform.rotation = Quaternion.Euler(0f, angle, 0f);

          Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
          controller.Move(moveDir.normalized * speed * Time.deltaTime);

          //So that when player jumps while moving doesn't walk in air rather plays the falling animation
          if(inAir == true)
          {
            anim.SetBool("isRunning", false);

            anim.SetBool("jump", true);
            //anim.SetBool("inAirr", true);
            anim.SetBool("landed", true);
            powerJumpTimer = defaultPowerJumpTimer;
          }
          else
          {
            anim.SetBool("isRunning", true);
            anim.SetBool("inAirr", false);
            anim.SetBool("jump", false);
            anim.SetBool("isIdle", false);
          }
        }
        else
        {
          running = false; 
          anim.SetBool("isRunning", false);

          if(isjumping)
          {
            anim.SetBool("isIdle", false);
          }
          else
          {
            anim.SetBool("isIdle", true);
          }
        }
      }


      //Applies gravity (See full code outside the Update void)
      applyGravity();

      //Applies jump buffer
      if (Input.GetButtonDown("Jump"))
      {

        jumpBufferCounter = jumpBufferTime;
        landTimerResett();
        isjumping = true;

        if(running)
        {
          anim.SetBool("isRunning", false);
          anim.SetBool("inAirr", true);
          anim.SetBool("jump", false);
        }
        else
        {
          anim.SetBool("jump", true);
          anim.SetBool("landed", true);
        }
      }
      else
      {
        jumpBufferCounter -= Time.deltaTime;
      }
    
      //Allows to jump with jump buffer
      if (coyoteTimeCounter > 0 && jumpBufferCounter > 0f)
      {
        jumpBufferCounter = 0f;
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity * Time.deltaTime);
      }

      if (Input.GetButtonDown("Jump") && velocity.y < 0f)
      {
        coyoteTimeCounter = 0f;
      }

      //Power Jump 
      if (canHoldQ && Input.GetKey(KeyCode.Q))
      {
        powerJumpTimer -= Time.deltaTime;
        anim.SetBool("holdingQ", true);
        if (powerJumpTimer <= 0 && isGrounded)
        {
          powerJumpAvailable = true;
          Debug.Log("Power Jump Available!");
        }
        else
        {
          powerJumpAvailable = false;
        }
      }

      if (Input.GetKeyUp(KeyCode.Q))
      {
        if(powerJumpTimer < defaultPowerJumpTimer)
        {
          powerJumpTimerRest();
          powerJumpAvailable = false;
          anim.SetBool("holdingQ", false);
          Debug.Log("Power Jump Unavailable!");
        }
        allowMove = false;
      }
      else
      {
        allowMove = true;
      }

      if(powerJumpAvailable && Input.GetButtonDown("Jump"))
      {
        Debug.Log("PowerJump");
        powerJumpTimerRest();
        settingPowerJumpFalsee();
        powerJumpAvailable = false;
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity * powerJumpPower * Time.deltaTime);
        anim.SetBool("holdingQ", false);
        anim.SetBool("powerJump", true);
        anim.SetBool("jump", false);
        anim.SetBool("landed", true);
        powerJumped = true;
        justPowerJumped = true;
      }

      //Dash Down
      if(inAir && Input.GetKeyDown(KeyCode.E))
      {
        dashDown = true;
        Debug.Log("Dash Down");
        anim.SetBool("powerjump", true);
        settingPowerJumpFalsee ();
      }

      if(dashDown == true)
      {
        velocity.y += dashDownGravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
      }

      if (inAir == false)
      {
        dashDown = false;
        applyGravity();
      }
    }

    //Applies Gravity
    void applyGravity()
    {
      velocity.y += gravity * Time.deltaTime;
      controller.Move(velocity * Time.deltaTime);
    }

    void powerJumpTimerRest()
    {
      powerJumpTimer = defaultPowerJumpTimer;
    }

    void justPowerJumpedTimerRest ()
    {
      justTouchedGround = justTouchedGroundRest;
    }

    void settingPowerJumpFalsee ()
    {
      settingPowerJumpFalse = settingPowerJumpFalseReset;
    }

    void landTimerResett()
    {
      landTimer = landTimerReset;
    }
}

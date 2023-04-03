using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMvt : MonoBehaviour
{
    [Header("Movement Details")]
    [SerializeField] private float speed = 8.0f;
    private float direction;
    private bool facingRight = true;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    [Header("Jump Details")]
    [SerializeField]private float jumpForce = 10.0f;
    private bool stoppedJumping;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f,16f);
 
    [SerializeField]private float jumpsLeft;
    [SerializeField]private float maxJumps = 2f;
    
    [Header("Ground Details")]
    [SerializeField]private float radOfCircle;
    [SerializeField]private LayerMask groundMask;
    [SerializeField]private bool grounded;
    [SerializeField]private Transform groundCheck;

    [Header("Wall Details")]
    [SerializeField]private Transform wallCheck;
    [SerializeField]private LayerMask wallLayer;
    
    [Header("Rigidbody, Animator")]
    private Rigidbody2D rb; 
    private Animator myAnimator; 
    


    //getting the rigid body and animator components.
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();

        jumpsLeft = maxJumps;  

    }

    //method that repeats every frame used to check if the player is touching the ground and is facing the right direction.
    //handles the movement of the player.
    private void Update()
    {
        //if the player is on the ground the falling anim will be false. 
        if (IsGrounded())
        {
            myAnimator.SetBool("falling", false);
            myAnimator.SetBool("walled", false);
            jumpsLeft = maxJumps;

            coyoteTimeCounter = coyoteTime;
        }
        else 
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        //when the player is falling play anims
        if(rb.velocity.y < 0f)
        {
            if(IsWalled())
            {
                WallSlide();
            }
            else
            {
                myAnimator.SetBool("walled", false);
                myAnimator.SetBool("falling", true);
            }

            //this is added to make sure the falling animation isnt stuck if the player holds the button while moving.
            if(IsGrounded())
            {
                myAnimator.SetBool("falling", false);
                myAnimator.SetBool("walled", false);
                myAnimator.ResetTrigger("jump"); 
                isWallSliding = false;
            }
        }
        //making sure the player is facing the correct direction.
        if (!facingRight && direction > 0f)
        {
            Flip();
        }
        //making sure the player is facing the correct direction.
        else if(facingRight && direction < 0f)
        {
            Flip();
        }
        //controlling the movement of the player.
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        
    }

    private void FixedUpdate()
    {
        
    }

    //using the new input system to control the player jump.
    public void Jump(InputAction.CallbackContext context)
    {
        //when jump is pressed and jumps left is more than 0.
        if (context.performed && jumpsLeft > 0f && !isWallSliding)
        {
            //adds a velocity (jump force) to the y value of the rigid body.
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            myAnimator.SetTrigger("jump");
            myAnimator.SetBool("walled", false);
            myAnimator.SetBool("falling", false);
            
            //the coyote timer makes it so there is some leaniency with jumping if you have just left the ground and the jump button is pressed the player will still jump. 
            //taking away 1 jump from jumps left.   
            if(coyoteTimeCounter <= 0f)
            {
                jumpsLeft -= 1f;
            }
        } 
        else if (context.performed && isWallSliding)
        {

        }
        //when jump is canceled.
        else if (context.canceled && rb.velocity.y > 0f)
        {
            //allowing the player to jump higher by pressing jump for longer and lower by pressing it for a short time.
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            myAnimator.ResetTrigger("jump");
            myAnimator.SetBool("falling", true);

            coyoteTimeCounter = 0f;
        }


    }

    //finding the direction the player is trying to move
    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().x;
        myAnimator.SetFloat("speed", Mathf.Abs(direction));

        
    }
    //bool to check if the player is on the ground. returns true or false.
    private bool IsGrounded()
    {
        //drawing a small circle under the rigid body to check if its touching the ground mask. if it is return true. if not return false.
        return Physics2D.OverlapCircle(groundCheck.position, radOfCircle, groundMask);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, radOfCircle, wallLayer);
    }

    private void WallSlide()
    {
        if(IsWalled() && !IsGrounded() && (direction > 0f || direction < 0f)) 
        {
            isWallSliding = true;
            myAnimator.SetBool("walled", true);
            myAnimator.ResetTrigger("jump"); 
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            myAnimator.SetBool("walled", false);
            myAnimator.ResetTrigger("jump"); 
        }
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }
    //method used to change the direction a rigid body is facing 
    private void Flip()
    {
            facingRight = !facingRight;
            
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
    }

    //used to draw a gizmo that is visible to the editor but not in game.
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, radOfCircle);
        Gizmos.DrawSphere(wallCheck.position, radOfCircle);
    }
    
}

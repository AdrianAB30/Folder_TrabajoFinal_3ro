using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private float speed;
    [SerializeField] private float speedRunning;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private Transform checkGround;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpCooldownTime = 0.1f;
    [SerializeField] private float jumpTimeCollider = 0.5f;
    private float originalSpeed;
    private Vector2 movement;

    private bool canJump = true;
    private bool hasLanded = false;
    private bool isJumping;
    private bool isGrounded;
    private bool canMove = true;
    private bool isFastRunning;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private CapsuleCollider myCollider;
    private PlayerInput playerInput;
    private Animator myAnimator;

    private void Awake()
    {    
        myCollider = GetComponent<CapsuleCollider>();
        myRBD = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        myAnimator = GetComponent<Animator>();     
    }
    private void Start()
    {
        originalSpeed = speed;
    }
    private void FixedUpdate()
    {
        AnimationsPlayer();
        RotatePlayer();
        ApplyPhysics();
        CheckGround();
    }
    private void ApplyPhysics()
    {
        myRBD.velocity = new Vector3(movement.x * speed, myRBD.velocity.y, movement.y * speed);

        if (isJumping && isGrounded && canJump)
        {
            myRBD.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = false;
            hasLanded = false;
            canJump = false;
            StartCoroutine(JumpCooldownAndCollider());
            StartCoroutine(JumpAdjustCollider());
        }
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        if(canMove)
        {
            movement = context.ReadValue<Vector2>();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canJump)
        {
            isJumping = true;
            myAnimator.SetBool("isJumpNormal",true);
        }
    }
    private IEnumerator JumpAdjustCollider()
    {
        myCollider.height = 4f;
        myCollider.radius = 0.8f;
        myCollider.center = new Vector3(0.33f, 3.3f, 0.4f);
        yield return new WaitForSeconds(jumpTimeCollider);
        myCollider.height = 5.3f;
        myCollider.radius = 0.88f;
        myCollider.center = new Vector3(0f, 2.7f, 0.1f);
    }
    private IEnumerator JumpCooldownAndCollider()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldownTime); 
        canJump = true;
        if (isGrounded)
        {
            myAnimator.SetBool("isJumpNormal", false); 
        }
    }
    public void OnRunning(InputAction.CallbackContext context)
    {
        if (isGrounded && movement.magnitude > 0)
        {
            if (context.performed)
            {
                isFastRunning = true;
                speed = originalSpeed + speedRunning;
                myAnimator.SetBool("isRunning", true);
            }
            else if (context.canceled)
            {
                isFastRunning = false;
                speed = originalSpeed;
                myAnimator.SetBool("isRunning", false);
            }
        }
        else
        {
            speed = originalSpeed;
            myAnimator.SetBool("isRunning", false);
        }
    }
    private void AnimationsPlayer()
    {
        myAnimator.SetFloat("X", movement.x);
        myAnimator.SetFloat("Y", movement.y);
    }
    private void RotatePlayer()
    {
        if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -95, 0);
        }
        else if (movement.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 95, 0);
        }
        if (movement.y < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (movement.y > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(checkGround.transform.position, Vector3.down, groundDistance, groundLayer);

        if(isGrounded && !isJumping)
        {
            hasLanded = true;
            myAnimator.SetBool("isJumpNormal",false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(checkGround.transform.position, checkGround.transform.position + Vector3.down * groundDistance);
    }
}
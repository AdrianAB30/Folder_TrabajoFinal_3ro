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
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpTimeCollider = 0.5f;
    private float jumpCooldownTime = 0.1f;
    private float attackCooldown = 1.3f;
    private float originalSpeed;
    private Vector2 movement;

    [Header("References Managers")]
    [SerializeField] private LifeManager lifeManager;

    private bool isTakingDamage = false;
    public bool canJump = true;
    private bool isJumping;
    private bool isGrounded;
    private bool canMove = true;
    private bool isFastRunning;
    private bool isAttacking;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private CapsuleCollider myCollider;
    private CapsuleCollider colliderFeets;
    private PlayerInput playerInput;
    private Animator myAnimator;


    private void OnEnable()
    {
        LifeManager.OnPlayerDamage += TakeDamage;
    }
    private void OnDisable()
    {
        LifeManager.OnPlayerDamage -= TakeDamage;
    }
    private void Awake()
    {    
        myCollider = GetComponent<CapsuleCollider>();
        colliderFeets = GetComponentInChildren<CapsuleCollider>();
        myRBD = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        myAnimator = GetComponent<Animator>();     
    }
    private void Start()
    {
        isGrounded = true;
        isJumping = false;
        isAttacking = false;
        originalSpeed = speed;

        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
        myAnimator.SetBool("isRunning", false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage();
        }
    }
    private void FixedUpdate()
    {
        CheckGround();
        AnimationsPlayer();
        RotatePlayer();
        ApplyPhysics();
    }
    private void ApplyPhysics()
    {
        myRBD.velocity = new Vector3(movement.x * speed, myRBD.velocity.y, movement.y * speed);

        if (isJumping && isGrounded && canJump)
        {
            myRBD.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = false;
            canJump = false;
            StartCoroutine(JumpCooldown());
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
            myAnimator.SetBool("isJumpNormal", true);
            myAnimator.SetBool("isInGround", true);
        }
        else
        {
            IsFalling();
        }

    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking && movement.magnitude == 0)
        {
            isAttacking = true;
            StartCoroutine(AttackCooldown());
            myAnimator.SetBool("isAttack",true);
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
    private void IsFalling()
    {
        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
    }
    private IEnumerator AttackCooldown()
    {
        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
        myAnimator.SetBool("isRunning", false);
        canMove = false;
        yield return new WaitForSeconds(attackCooldown);
        canMove = true;
        speed = originalSpeed;
        myAnimator.SetBool("isAttack", false); 
        isAttacking = false;
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
    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldownTime); 
        canJump = true;
        if (isGrounded)
        {
            myAnimator.SetBool("isJumpNormal", false); 
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
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            myAnimator.SetBool("isJumpNormal", false);
            myAnimator.SetBool("isInGround", true);
            Debug.Log("Player landed");
        }
        else if (!isGrounded)
        {
            myAnimator.SetBool("isInGround", true);
            Debug.Log("Player is falling");
        }
    }
    private void TakeDamage()
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            lifeManager.DamageToPlayer();
            isTakingDamage = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }
}
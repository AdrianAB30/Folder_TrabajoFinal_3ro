using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private float speed;
    [SerializeField] private float speedRunning;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpTimeCollider;
    [SerializeField] private GameObject checkGround;
    private float attackCooldown = 1.3f;
    private float originalSpeed;
    private Vector2 movement;

    [Header("References Managers")]
    [SerializeField] private LifeManager lifeManager;

    private bool wasGrounded = false;
    private bool isTakingDamage = false;
    public bool canJump = true;
    private bool isJumping;
    private bool isGrounded;
    private bool canMove = true;
    private bool isFastRunning;
    private bool isAttacking;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private BoxCollider myCollider;
    private PlayerInput playerInput;
    private Animator myAnimator;


    //Eventos
    public event Action OnJumpColl;
    public event Action OnFallingColl;


    private void OnEnable()
    {
        LifeManager.OnPlayerDamage += TakeDamage;
        OnJumpColl += JumpCollider;
        OnFallingColl += FallingCollider;
    }
    private void OnDisable()
    {
        LifeManager.OnPlayerDamage -= TakeDamage;
        OnJumpColl -= JumpCollider;
        OnFallingColl -= FallingCollider;
    }
    private void Awake()
    {    
        myCollider = GetComponent<BoxCollider>();
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
            OnJumpColl?.Invoke();
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
    private void JumpCollider()
    {
        myCollider.center = new Vector3(0.025f, 2.1f, 0.2f);
        myCollider.size = new Vector3(1.72f, 3.6f, 1.6f);
    }
    private void FallingCollider()
    {
        myCollider.size = new Vector3(1.72f, 5.1f, 1.55f);
        myCollider.center = new Vector3(0.025f, 2.78f, 0.2f);
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

        if (isGrounded && !wasGrounded) 
        {
            myAnimator.SetBool("isJumpNormal", false);
            myAnimator.SetBool("isInGround", true);
            canJump = true;
            OnFallingColl?.Invoke();
        }
        else if (!isGrounded)
        {
            myAnimator.SetBool("isInGround", false);
            myAnimator.SetBool("isJumpNormal", false);
            canJump = false;
            Debug.Log("Player is falling");
        }

        wasGrounded = isGrounded;
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

        Gizmos.DrawLine(checkGround.transform.position, checkGround.transform.position + Vector3.down * groundDistance);
    }
}
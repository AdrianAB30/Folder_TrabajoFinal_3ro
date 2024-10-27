using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject checkGround;
    private bool isAttacking = false;
    private Vector3 jumpingCenter = new Vector3(0.025f, 2.1f, 0.2f);
    private Vector3 jumpingSize = new Vector3(1.72f, 3f, 1.6f);

    private Vector3 fallingCenter = new Vector3(0.025f, 2.7f, 0.2f);
    private Vector3 fallingSize = new Vector3(1.72f, 5f, 1.55f);

    [Header("References Managers")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private InventoryPlayer inventoryPlayer;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private BoxCollider myCollider;
    private Animator myAnimator;

    //Eventos
    public event Action OnJumpColl;
    public event Action OnFallingColl;

    private void OnEnable()
    {
        LifeManager.OnPlayerDamage += TakeDamage;
        OnJumpColl += JumpCollider;
        OnFallingColl += FallingCollider;
        InputHandler.OnMovementInput += OnMovement;
        InputHandler.OnJumpInput += OnJump;
        InputHandler.OnAttackInput += OnAttack;
        InputHandler.OnRunningInput += OnRunning;
        InputHandler.OnCoverInput += OnCovering;
        InputHandler.OnRollingInput += OnRolling;
    }
    private void OnDisable()
    {
        LifeManager.OnPlayerDamage -= TakeDamage;
        OnJumpColl -= JumpCollider;
        OnFallingColl -= FallingCollider;
        InputHandler.OnMovementInput -= OnMovement;
        InputHandler.OnJumpInput -= OnJump;
        InputHandler.OnAttackInput -= OnAttack;
        InputHandler.OnRunningInput -= OnRunning;
        InputHandler.OnCoverInput -= OnCovering;
        InputHandler.OnRollingInput -= OnRolling;
    }
    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myRBD = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
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

    #region Physics

    private void ApplyPhysics()
    {
        Vector3 velocity = myRBD.velocity;
        velocity.x = playerData.movement.x * playerData.speed;
        velocity.z = playerData.movement.y * playerData.speed;

        if (!playerData.isGrounded && myRBD.velocity.y < 0)
        {
            velocity.y += Physics.gravity.y * playerData.forceFalling * Time.fixedDeltaTime;
        }

        myRBD.velocity = velocity;

        if (playerData.isJumping && playerData.isGrounded && playerData.canJump)
        {
            myRBD.AddForce(Vector3.up * playerData.jumpForce, ForceMode.Impulse);
            playerData.isJumping = false;
            playerData.canJump = false;
        }
    }

    #endregion

    #region MOVEMENT

    private void OnMovement(Vector2 movementInput)
    {
        if (playerData.canMove && !playerData.isRolling)
        {
            playerData.movement = movementInput;

            if (playerData.movement.magnitude == 0)
            {
                playerData.speed = playerData.originalSpeed;
                myAnimator.SetBool("isRunning", false);
                myAnimator.SetBool("isAttack", false);
            }
        }
    }
    private void OnJump()
    {
        if (playerData.isGrounded && playerData.canJump && !playerData.isAttacking)
        {
            playerData.isJumping = true;
            myAnimator.SetBool("isJumpNormal", true);
            myAnimator.SetBool("isInGround", false);
            OnJumpColl?.Invoke();
        }
        else
        {
            IsFalling();
        }
    }
    private void OnAttack()
    {
        if (!isAttacking && !playerData.isJumping && playerData.movement.magnitude == 0 && !playerData.isCovering && playerData.isGrounded)
        {
            isAttacking = true;
            playerData.canJump = false;
            playerData.isAttacking = true;
            StartCoroutine(AttackCooldown());
            myAnimator.SetBool("isAttack", true);
        }
    }
    private void OnCovering(bool isCovering)
    {
        if (!playerData.isJumping && !playerData.isAttacking && !playerData.isRolling)
        {
            playerData.isCovering = isCovering;
            myAnimator.SetBool("isCovering", isCovering);

            if (isCovering)
            {
                playerData.canMove = false;
                playerData.canJump = false;
                playerData.isRolling = false;
                playerData.speed = 0f;
                playerData.movement = Vector2.zero;
            }
            else
            {
                playerData.canMove = true;
                playerData.canJump = true;
                playerData.speed = playerData.originalSpeed;
            }
        }
    }
    private void OnRunning(bool isRunning)
    {
        if (playerData.isGrounded && playerData.movement.magnitude > 0)
        {
            if (isRunning)
            {
                playerData.speed = playerData.originalSpeed + playerData.speedRunning;
                myAnimator.SetBool("isRunning", true);
            }
            else
            {
                playerData.speed = playerData.originalSpeed;
                myAnimator.SetBool("isRunning", false);
            }
        }
        else
        {
            playerData.speed = playerData.originalSpeed;
            myAnimator.SetBool("isRunning", false);
        }
    }
    private void OnRolling(bool isRolling)
    {
        if (isRolling)
        {
            if (playerData.isGrounded && !playerData.isCovering && !playerData.isJumping && !playerData.isAttacking)
            {
                playerData.isRolling = true;
                playerData.canMove = false;
                playerData.canJump = false;
                playerData.isAttacking = false;
                myAnimator.SetBool("isRolling", true);

                StartCoroutine(Rolling());
            }
        }
    }
    private void JumpCollider()
    {
        myCollider.center = jumpingCenter;
        myCollider.size = jumpingSize;
    }
    private void FallingCollider()
    {
        myCollider.center = fallingCenter;
        myCollider.size = fallingSize;
    }
    private void IsFalling()
    {
        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
    }
    #endregion

    #region CORRUTINAS
    private IEnumerator AttackCooldown()
    {
        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
        myAnimator.SetBool("isRunning", false);
        playerData.canMove = false;

        yield return new WaitForSeconds(playerData.attackCooldown);

        isAttacking = false;
        playerData.canMove = true;
        playerData.speed = playerData.originalSpeed;
        myAnimator.SetBool("isAttack", false);
        playerData.isAttacking = false;
        playerData.canJump = true;
    }
    private IEnumerator Rolling()
    {
        myCollider.center = new Vector3(0.025f, 1.15f, 0.7f);
        myCollider.size = new Vector3(1.72f, 1.8f, 4f);
        Vector3 rollDirection = new Vector3(playerData.movement.x, 0, playerData.movement.y).normalized;

        if (rollDirection.magnitude > 0 && !playerData.isRolling)
        {
            myRBD.AddForce(rollDirection * playerData.rollImpulse, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f);

        myCollider.center = new Vector3(0.025f, 2.78f, 0.2f);
        myCollider.size = new Vector3(1.72f, 5.1f, 1.55f);
        playerData.isRolling = false;
        myAnimator.SetBool("isRolling", false);

        playerData.canMove = true;
        playerData.canJump = true;

        playerData.movement = Vector2.zero;
    }
    #endregion

    #region ANIMATIONS
    private void AnimationsPlayer()
    {
        myAnimator.SetFloat("X", playerData.movement.x);
        myAnimator.SetFloat("Y", playerData.movement.y);

        if (playerData.isCovering)
        {
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isAttack", false);
            return;
        }
    }
    private void RotatePlayer()
    {
        Vector3 targetDirection = new Vector3(playerData.movement.x, 0, playerData.movement.y).normalized;

        if (targetDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerData.rotationSpeed * Time.deltaTime);
        }
    }
    #endregion 

    private void CheckGround()
    {
        playerData.isGrounded = Physics.Raycast(checkGround.transform.position, Vector3.down, playerData.groundDistance, playerData.groundLayer);

        if (playerData.isGrounded && !playerData.wasGrounded)
        {
            myAnimator.SetBool("isJumpNormal", false);
            myAnimator.SetBool("isInGround", true);
            playerData.canJump = true;
            playerData.isJumping = false;
            OnFallingColl?.Invoke();

            if (playerData.isCovering)
            {
                playerData.canMove = false;
            }
        }
        else if (!playerData.isGrounded)
        {
            myAnimator.SetBool("isInGround", false);
            playerData.canJump = false;

            if (playerData.isCovering)
            {
                OnCovering(false);
            }
        }
        playerData.wasGrounded = playerData.isGrounded;
    }
    private void TakeDamage()
    {
        if (!playerData.isTakingDamage)
        {
            playerData.isTakingDamage = true;
            lifeManager.DamageToPlayer();
            playerData.isTakingDamage = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GroundBow"))
        {
            Debug.Log("Has recogido el arco del suelo.");
            inventoryPlayer.AddWeapon(inventoryPlayer.GetBow());
            inventoryPlayer.ActivateBow();
            inventoryPlayer.ActivateQuiver();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("GroundQuiver")) 
        {
            Debug.Log("Has recogido la funda del suelo.");
            other.gameObject.SetActive(false);
        }
    }
}
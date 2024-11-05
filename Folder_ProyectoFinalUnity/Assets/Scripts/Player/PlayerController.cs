using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject checkGround;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance;
    private float originalSpeed = 6f;
    private float rotationSpeed = 10f;
    private Vector2 movement;
    private Vector3 jumpingCenter = new Vector3(0.025f, 2.1f, 0.2f);
    private Vector3 jumpingSize = new Vector3(1.72f, 4f, 1.6f);

    private Vector3 fallingCenter = new Vector3(0.025f, 2.7f, 0.2f);
    private Vector3 fallingSize = new Vector3(1.72f, 5f, 1.55f);

    [Header("References Managers")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private InventoryPlayer inventoryPlayer;
    [SerializeField] private Particle_Class particleClass;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SfxSounds sfxSounds;

    [Header("References NPCS")]
    [SerializeField] private HerreraController herrera;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private BoxCollider myCollider;
    private Animator myAnimator;
    private AudioSource myAudioSource;

    [Header("Player Booleans")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isCovering = false;
    [SerializeField] private bool isRolling = false;
    [SerializeField] private bool isJumping = false;
    private bool wasGrounded = false;
    private bool isPlayerRunning = false;
    private bool isTakingDamage = false;
    private bool isGrounded = true;

    //Eventos
    public event Action OnJumpColl;
    public event Action OnFallingColl;
    public static event Action OnStartRollingParticle;
    public static event Action OnEndRollingParticle;
    public static event Action OnBowCollected;
    public static event Action OnSwordCollected;

    private void OnEnable()
    {
        lifeManager.OnPlayerDamage += TakeDamage;
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
        lifeManager.OnPlayerDamage -= TakeDamage;
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
        myAudioSource = GetComponent<AudioSource>();
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
            TakeDamage(1);
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
        velocity.x = movement.x * playerData.walkspeed;
        velocity.z = movement.y * playerData.walkspeed;
        if (!isGrounded && myRBD.velocity.y < 0)
        {
            velocity.y += Physics.gravity.y * playerData.forceFalling * Time.fixedDeltaTime;
        }

        myRBD.velocity = velocity;

        if (isJumping && isGrounded && canJump)
        {
            myRBD.AddForce(Vector3.up * playerData.jumpForce, ForceMode.Impulse);
            isJumping = false;
            canJump = false;
        }
    }

    #endregion

    #region MOVEMENT

    private void OnMovement(Vector2 movementInput)
    {
        if (canMove && !isRolling)
        {
            movement = movementInput;
            if (movement.magnitude == 0)
            {
                playerData.walkspeed = originalSpeed;
                myAnimator.SetBool("isRunning", false);
                myAnimator.SetBool("isAttack", false);
            }
        }
    }
    private void OnJump()
    {
        if (isGrounded && canJump && !isAttacking && !isCovering)
        {
            isJumping = true;
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
        if (!isAttacking && !isJumping && movement.magnitude == 0 && !isCovering && isGrounded)
        {
            isAttacking = true;
            canJump = false;
            isAttacking = true;
            StartCoroutine(AttackCooldown());
            myAnimator.SetBool("isAttack", true);
        }
    }
    private void OnCovering(bool isCoveringPlayer)
    {
        if (!isJumping && !isAttacking && !isRolling)
        {
            isCovering = isCoveringPlayer;
            myAnimator.SetBool("isCovering", isCovering);

            if (isCovering)
            {
                canMove = false;
                canJump = false;
                isRolling = false;
                playerData.walkspeed = 0f;
                movement = Vector2.zero;
            }
            else
            {
                canMove = true;
                canJump = true;
                playerData.walkspeed = originalSpeed;
            }
        }
    }
    private void OnRunning(bool isRunning)
    {
        isPlayerRunning = isRunning;
        if (isGrounded && movement.magnitude > 0)
        {
            if (isRunning)
            {
                playerData.walkspeed = originalSpeed + playerData.speedRunning;

                myAnimator.SetBool("isRunning", true);
                canJump = false;
            }
            else
            {
                playerData.walkspeed = originalSpeed;
                myAnimator.SetBool("isRunning", false);
                canJump = true; 
            }
        }
        else
        {
            playerData.walkspeed = originalSpeed;
            myAnimator.SetBool("isRunning", false);
            canJump = true;
        }
    }
    private void OnRolling(bool isRollingPlayer)
    {
        isRolling = isRollingPlayer;
        if (isRollingPlayer)
        {
            if (isGrounded && isPlayerRunning && movement.magnitude > 0 && !isCovering && !isJumping && !isAttacking)
            {
                isRolling = true;
                canMove = false;
                canJump = false;
                isAttacking = false;
                myAnimator.SetBool("isRolling", true);
                OnStartRollingParticle?.Invoke();
                myAudioSource.PlayOneShot(sfxSounds.soundSfx[2]);
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
        canMove = false;

        yield return new WaitForSeconds(playerData.attackCooldown);

        isAttacking = false;
        canMove = true;
        playerData.walkspeed = originalSpeed;
        myAnimator.SetBool("isAttack", false);
        isAttacking = false;
        canJump = true;
    }
    private IEnumerator Rolling()
    {
        myCollider.center = new Vector3(0.025f, 1.15f, 0.7f);
        myCollider.size = new Vector3(1.72f, 1.8f, 4f);
        Vector3 rollDirection = new Vector3(movement.x, 0, movement.y).normalized;
        if (rollDirection.magnitude > 0 && !isRolling)
        {
            myRBD.AddForce(rollDirection * playerData.rollImpulse, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f);
        myCollider.center = new Vector3(0.025f, 2.78f, 0.2f);
        myCollider.size = new Vector3(1.72f, 5.1f, 1.55f);
        isRolling = false;
        myAnimator.SetBool("isRolling", false);
        OnEndRollingParticle?.Invoke();

        canMove = true;
        canJump = true;

        movement = Vector2.zero;
    }
    #endregion

    #region ANIMATIONS
    private void AnimationsPlayer()
    {
        myAnimator.SetFloat("X", movement.x);
        myAnimator.SetFloat("Y", movement.y);

        if (isCovering)
        {
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isAttack", false);
            return;
        }
    }
    private void RotatePlayer()
    {
        Vector3 targetDirection = new Vector3(movement.x, 0, movement.y).normalized;

        if (targetDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion 

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(checkGround.transform.position, Vector3.down, groundDistance, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            myAnimator.SetBool("isJumpNormal", false);
            myAnimator.SetBool("isInGround", true);
            canJump = true;
            isJumping = false;
            OnFallingColl?.Invoke();

            if (isCovering)
            {
                canMove = false;
            }
        }
        else if (!isGrounded)
        {
            myAnimator.SetBool("isInGround", false);
            canJump = false;

            if (isCovering)
            {
                OnCovering(false);
            }
        }
        wasGrounded = isGrounded;
    }
    public void TakeDamage(int damageAmount)
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            lifeManager.DamageToPlayer(damageAmount);
            isTakingDamage = false;
        }
    }
    public void TriggerEquipEnd()
    {
        if (inventoryPlayer != null)
        {
            inventoryPlayer.OnEquipAnimationEnd();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GroundBow"))
        {
            Debug.Log("Has recogido el arco del suelo.");
            inventoryPlayer.AddWeapon(inventoryPlayer.GetBow());
            OnBowCollected?.Invoke();
            inventoryPlayer.ActivateBow();
            inventoryPlayer.ActivateQuiver();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("GroundQuiver")) 
        {
            Debug.Log("Has recogido la funda del suelo.");
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("GroundSword"))
        {
            Debug.Log("Has recogido la espada y escudo");
            inventoryPlayer.AddWeapon(inventoryPlayer.GetSwordAndShield());
            OnSwordCollected?.Invoke();
            inventoryPlayer.ActivateSword();
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Shield"))
        {
            other.gameObject.SetActive(false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(checkGround.transform.position, Vector3.down * groundDistance);
    }
}
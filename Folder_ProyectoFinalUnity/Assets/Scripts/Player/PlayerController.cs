using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector3 fallingCenter = new Vector3(0.025f, 2.75f, 0.2f);
    private Vector3 fallingSize = new Vector3(1.72f, 5f, 1.55f);

    [Header("References Managers")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private InventoryPlayer inventoryPlayer;
    [SerializeField] private Particle_Class particleClass;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SfxSounds sfxSounds;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameManager gameManager;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private BoxCollider myCollider;
    private Animator myAnimator;
    private AudioSource myAudioSource;

    [Header("Player Booleans")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isAttackSword = false;
    [SerializeField] private bool isAttackBow = false;
    [SerializeField] private bool isCovering = false;
    public bool isRolling = false;
    public bool isJumping = false;
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
    public static event Action<bool> OnAttackBowSpawner;
    public event Action OnPlayerStand;

    private void OnEnable()
    {
        lifeManager.OnPlayerDamage += TakeDamage;
        OnJumpColl += JumpCollider;
        OnFallingColl += FallingCollider;
        inputHandler.OnMovementInput += OnMovement;
        inputHandler.OnJumpInput += OnJump;
        inputHandler.OnAttackSwordInput += OnAttackSword;
        inputHandler.OnAttackBow += OnAttackBow;
        inputHandler.OnRunningInput += OnRunning;
        inputHandler.OnCoverInput += OnCovering;
        inputHandler.OnRollingInput += OnRolling;
    }
    private void OnDisable()
    {
        lifeManager.OnPlayerDamage -= TakeDamage;
        OnJumpColl -= JumpCollider;
        OnFallingColl -= FallingCollider;
        inputHandler.OnMovementInput -= OnMovement;
        inputHandler.OnJumpInput -= OnJump;
        inputHandler.OnAttackSwordInput -= OnAttackSword;
        inputHandler.OnAttackBow -= OnAttackBow;
        inputHandler.OnRunningInput -= OnRunning;
        inputHandler.OnCoverInput -= OnCovering;
        inputHandler.OnRollingInput -= OnRolling;
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
        StartCoroutine(StandCooldown());
        playerData.Stamina = 100f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage(1);
        }
        StaminaLogic();
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
            StartCoroutine(JumpCooldown());

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
                IncreaseStamina(Time.deltaTime * 5f);
                playerData.walkspeed = originalSpeed;
                myAnimator.SetBool("isRunning", false);
                myAnimator.SetBool("isAttack", false);
            }
        }
    }
    private void OnJump()
    {
        if (isGrounded && canJump && !isAttackSword && !isCovering && playerData.Stamina > 10 && !isPlayerRunning)
        {
            DecreaseStamina(8);
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
    private void OnAttackSword()
    {
        if (!isAttackSword && !isJumping && movement.magnitude == 0 && !isCovering && isGrounded && playerData.Stamina > 10)
        {
            DecreaseStamina(8);
            isAttackSword = true;
            canJump = false;
            canMove = false;
            myAnimator.SetBool("isAttack", true);
            StartCoroutine(AttackCooldown());
        }
    }
    private void OnAttackBow(bool isPlayerAttacking)
    {
        if (isPlayerAttacking)
        {
            if (!isAttackBow && !isJumping && movement.magnitude == 0 && !isCovering && isGrounded)
            {
                isAttackBow = true;
                myAnimator.SetBool("isAttackBow",true);
                OnAttackBowSpawner?.Invoke(false);
            }
        }
        else
        {
            isAttackBow = false;
            myAnimator.SetBool("isAttackBow", false);
            canJump = true;
            canMove = true;
            OnAttackBowSpawner?.Invoke(true);
        }
    }
    private void OnCovering(bool isCoveringPlayer)
    {
        if (!isJumping && !isAttackSword && !isRolling)
        {
            isCovering = isCoveringPlayer;
            myAnimator.SetBool("isCovering", isCovering);

            if (isCovering)
            {
                IncreaseStamina(Time.deltaTime * 5f);
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
        if (playerData.Stamina > 10f && isRunning && isGrounded && movement.magnitude > 0)
        {
            isPlayerRunning = true;
            playerData.walkspeed = originalSpeed + playerData.speedRunning;
            myAnimator.SetBool("isRunning", true);
            canJump = false;
        }
        else
        {
            isPlayerRunning = false;
            playerData.walkspeed = originalSpeed;
            myAnimator.SetBool("isRunning", false);
            canJump = true;
        }
    }
    private void OnRolling()
    {
        if (isRolling) return;

        if (isGrounded && isPlayerRunning && movement.magnitude > 0 && !isCovering && !isJumping && !isAttackSword && playerData.Stamina > 10)
        {
            DecreaseStamina(8);
            isRolling = true;
            canMove = false;
            canJump = false;
            isAttackSword = false;
            myAnimator.SetBool("isRolling", true);
            OnStartRollingParticle?.Invoke();
            myAudioSource.PlayOneShot(sfxSounds.soundSfx[2]);
            StartCoroutine(Rolling());
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
    private IEnumerator Rolling()
    {
        myCollider.center = new Vector3(0.025f, 1.15f, 0.7f);
        myCollider.size = new Vector3(1.72f, 1.8f, 4f);
        Vector3 rollDirection = new Vector3(movement.x, 0, movement.y).normalized;
        if (rollDirection.magnitude > 0 && isRolling)
        {
            myRBD.AddForce(rollDirection * playerData.rollImpulse * Time.deltaTime, ForceMode.Impulse);
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
    private IEnumerator JumpCooldown()
    {
        inputHandler.canHandleInputJump = false;
        canJump = false;
        yield return new WaitForSeconds(1f);
        inputHandler.canHandleInputJump = true;
        canJump = true;
    }
    private IEnumerator AttackCooldown()
    {
        myAnimator.SetBool("isJumpNormal", false);
        myAnimator.SetBool("isInGround", true);
        myAnimator.SetBool("isRunning", false);
        isAttackSword = true;
        canMove = false;

        yield return new WaitForSeconds(playerData.attackCooldown);

        isAttackSword = false;
        canMove = true;
        playerData.walkspeed = originalSpeed;
        myAnimator.SetBool("isAttack", false);
        canJump = true;
    }
    private IEnumerator StandCooldown()
    {
        canJump = false;
        canMove = false;
        inputHandler.canHandleInput = false;
        inputHandler.canHandleInputJump = false;
        yield return new WaitForSeconds(3f);
        myAnimator.SetBool("isStand", true);
        OnPlayerStand?.Invoke();
        yield return new WaitForSeconds(4f);
        inputHandler.canHandleInput = true;
        inputHandler.canHandleInputJump = true;
        canJump = true;
        canMove = true;
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
        else if (other.CompareTag("Perder"))
        {
            gameManager.LooseGame();
        }
    }
    private void IncreaseStamina(float amount)
    {
        playerData.Stamina = Mathf.Min(playerData.Stamina + amount, 100);
        uiManager.UpdateStaminaBar();
    }
    private void DecreaseStamina(float amount)
    {
        if (gameManager.IsOptionsMenuActive) return;
        playerData.Stamina = Mathf.Max(playerData.Stamina - amount, 0);
        uiManager.UpdateStaminaBar();
    }
    private void StaminaLogic()
    {
        if (isPlayerRunning && playerData.Stamina > 0)
        {
            DecreaseStamina(Time.deltaTime * 10f);
        }
        else if (movement.magnitude == 0 || (movement.magnitude > 0 && !isPlayerRunning))
        {
            IncreaseStamina(Time.deltaTime * 5f);
        }
        else if (playerData.Stamina <= 0)
        {
            isPlayerRunning = false;
            playerData.walkspeed = originalSpeed;
            myAnimator.SetBool("isRunning", false);
            canJump = true;
        }
    }
    public float GetMovementMagnitude()
    {
        return movement.magnitude;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(checkGround.transform.position, checkGround.transform.position + Vector3.down * groundDistance);
    }
}
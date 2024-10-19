using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDistance;
    [SerializeField] private Transform checkGround;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 movement;

    private bool isJumping;
    private bool isGrounded;
    private bool canMove;

    [Header("Player Components")]
    private Rigidbody myRBD;
    private Collider myCollider;
    private PlayerInput playerInput;

    private void Awake()
    {
        myCollider = GetComponentInChildren<Collider>();
        myRBD = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void FixedUpdate()
    {
        ApplyPhysics();
        CheckGround();
    }
    private void ApplyPhysics()
    {
        myRBD.velocity = new Vector3(movement.x * speed, myRBD.velocity.y, movement.y * speed);
        
        if(isJumping && isGrounded)
        {
            myRBD.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
            isJumping = false;
        }
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded)
        {
            isJumping = true;
        }
    }
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(checkGround.transform.position,Vector3.down,groundDistance,groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(checkGround.transform.position, checkGround.transform.position + Vector3.down * groundDistance);
    }
}

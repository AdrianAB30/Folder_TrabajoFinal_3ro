using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Stats")]
    public Vector2 movement;
    public LayerMask groundLayer;
    public float speed;
    public float originalSpeed;
    public float speedRunning;
    public float forceFalling;
    public float jumpForce;
    public float groundDistance;
    public float attackCooldown;
    public float rollImpulse;
    public float rotationSpeed = 5f;

    [Header("Player Booleans")]
    public bool wasGrounded = false;
    public bool isTakingDamage = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool canJump = true;
    public bool isRunning = false;
    public bool canMove = true;
    public bool isAttacking = false;
    public bool isCovering = false;
    public bool isRolling = false;
}

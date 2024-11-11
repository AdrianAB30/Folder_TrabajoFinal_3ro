using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Stats")]
    public float walkspeed;
    public float life;
    public float speedRunning;
    public float forceFalling;
    public float jumpForce;
    public float attackCooldown;
    public float rollImpulse;
    public float standCooldown;
    public float Stamina;
}

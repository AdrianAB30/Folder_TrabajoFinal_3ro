using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArrowData", menuName = "ScriptableObjects/ArrowData", order = 5)]
public class ArrowData : ScriptableObject
{
    public float maxFallRotationX;
    public float maxFallRotationZ;
    public float damage;
    public float shootForce;
    public float timeToDestroy;
}

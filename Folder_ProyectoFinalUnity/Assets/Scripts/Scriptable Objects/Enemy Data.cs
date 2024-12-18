using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Enemy Data", order = 4)]
public class EnemyData : ScriptableObject
{
    public float speedChaseMultiplier;
    public int maxLife;
    public int pushingForceHit;
    public Vector3 directionPushing;

    public int GetMaxLife()
    {
        return maxLife;
    }
}

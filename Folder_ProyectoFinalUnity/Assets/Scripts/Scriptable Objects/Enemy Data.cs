using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Enemy Data", order = 4)]
public class EnemyData : ScriptableObject
{
    public float watingTime;
    public float forceRotateEnemy;
    public float maxLife;
}

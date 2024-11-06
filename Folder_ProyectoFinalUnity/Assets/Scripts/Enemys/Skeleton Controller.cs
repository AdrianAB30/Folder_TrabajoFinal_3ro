using System.Collections;
using UnityEngine;

public class SkeletonController : RoutePatrolRandom
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject[] nodes;
}

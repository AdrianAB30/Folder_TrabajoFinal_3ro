using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogControl : RoutePatrolDefined
{
    [Header("Patrol Data Dog")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject[] nodes;

    public Transform playerTransform;
    private bool playerInRange;
    private float originalForce;
    private float originalRotate;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        SetNodesPatrol();
        originalForce = movementForce;
        originalRotate = npcData.forceRotateNpc;
    }
    private void Update()
    {
        if (playerInRange)
        {
            RotateNpcToPlayer();
        }
        else
        {
            npcData.forceRotateNpc = originalRotate;
        }
    }
    private void SetNodesPatrol()
    {
        SimpleLinkedList<GameObject> patrolPointsRandom = new SimpleLinkedList<GameObject>();
        patrolPointsRandom.InsertNodeAtEnd(nodes[0]);
        patrolPointsRandom.InsertNodeAtEnd(nodes[1]);
        patrolPointsRandom.InsertNodeAtEnd(nodes[2]);
        patrolPointsRandom.InsertNodeAtEnd(nodes[3]);
        patrolPointsRandom.InsertNodeAtEnd(nodes[4]);
        patrolPointsRandom.InsertNodeAtEnd(nodes[5]);

        graphManager.AddNode(nodes[0]);
        graphManager.AddNode(nodes[1]);
        graphManager.AddNode(nodes[2]);
        graphManager.AddNode(nodes[3]);
        graphManager.AddNode(nodes[4]);
        graphManager.AddNode(nodes[5]);

        graphManager.AddDirectedConnection(nodes[0], nodes[1]);
        graphManager.AddDirectedConnection(nodes[1], nodes[2]);
        graphManager.AddDirectedConnection(nodes[2], nodes[3]);
        graphManager.AddDirectedConnection(nodes[3], nodes[4]);
        graphManager.AddDirectedConnection(nodes[4], nodes[5]);

        SetPatrolRoute(patrolPointsRandom);
    }
    private void OnDrawGizmos()
    {
        if (nodes[0] != null && nodes[1] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[0].transform.position, nodes[1].transform.position);
        }
        if (nodes[1] != null && nodes[2] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[1].transform.position, nodes[2].transform.position);
        }
        if (nodes[2] != null && nodes[3] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[2].transform.position, nodes[3].transform.position);
        }
        if (nodes[3] != null && nodes[4] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[3].transform.position, nodes[4].transform.position);
        }
        if (nodes[4] != null && nodes[5] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(nodes[4].transform.position, nodes[5].transform.position);
        }

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            npcAnimator.SetBool("isInteract", true);
            movementForce = 0f;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            npcAnimator.SetBool("isInteract", false);
            movementForce = originalForce;
            playerInRange = false;
        }
    }
    private void RotateNpcToPlayer()
    {
        if (playerTransform != null)
        {
            npcData.forceRotateNpc = 0f;
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.DORotateQuaternion(lookRotation, 0.08f).SetEase(Ease.InOutSine);
            }
        }
    }
}

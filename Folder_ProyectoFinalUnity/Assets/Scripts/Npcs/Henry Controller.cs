using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HenryController : RoutePatrolRandom
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject node1;
    [SerializeField] private GameObject node2;
    [SerializeField] private GameObject node3;
    [SerializeField] private GameObject node4;

    [Header("NPC Dialogue")]
    [SerializeField] private GameObject dialogueMark;

    public Transform playerTransform;
    private bool playerInRange;
    private float originalForce;
    private float originalRotate;

    //Eventos
    public static event Action<bool,string> OnPlayerEnter;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        SetNodesPatrol();
        originalForce = npcData.movementForce;
        originalRotate = npcData.rotateNpc;
    }
    private void Update()
    {
        if (playerInRange)
        {
            RotateNpcToPlayer();
        }
        else
        {
            npcData.rotateNpc = originalRotate;
        }
    }
    private void SetNodesPatrol()
    {
        SimpleLinkedList<GameObject> patrolPointsRandom = new SimpleLinkedList<GameObject>();
        patrolPointsRandom.InsertNodeAtEnd(node1);
        patrolPointsRandom.InsertNodeAtEnd(node2);
        patrolPointsRandom.InsertNodeAtEnd(node3);
        patrolPointsRandom.InsertNodeAtEnd(node4);

        graphManager.AddNode(node1);
        graphManager.AddNode(node2);
        graphManager.AddNode(node3);
        graphManager.AddNode(node4);

        graphManager.AddBidirectionalConnections(node1, node2);
        graphManager.AddBidirectionalConnections(node2, node3);
        graphManager.AddBidirectionalConnections(node3, node4);
        graphManager.AddBidirectionalConnections(node4, node1);

        SetPatrolRoute(patrolPointsRandom);
    }
    private void OnDrawGizmos()
    {
        if (node1 != null && node2 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(node1.transform.position, node2.transform.position);
        }
        if (node2 != null && node3 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(node2.transform.position, node3.transform.position);
        }
        if (node3 != null && node4 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(node3.transform.position, node4.transform.position);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueMark.SetActive(false);
            npcRandomAnimator.SetBool("isInteract", true);
            OnPlayerEnter?.Invoke(true,"Henry");
            npcData.movementForce = 0f;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            npcRandomAnimator.SetBool("isInteract", false);
            OnPlayerEnter?.Invoke(false,"Henry");
            npcData.movementForce = originalForce;
            playerInRange = false;
        }
    }
    private void RotateNpcToPlayer()
    {
        if (playerTransform != null)
        {
            npcData.rotateNpc = 0f;
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }
    }
}

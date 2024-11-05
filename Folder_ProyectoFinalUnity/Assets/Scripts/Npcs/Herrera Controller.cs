using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class HerreraController : RoutePatrolDefined
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
        SimpleLinkedList<GameObject> patrolPoints = new SimpleLinkedList<GameObject>();
        patrolPoints.InsertNodeAtEnd(node1);
        patrolPoints.InsertNodeAtEnd(node2);
        patrolPoints.InsertNodeAtEnd(node3);
        patrolPoints.InsertNodeAtEnd(node4);

        graphManager.AddNode(node1);
        graphManager.AddNode(node2);
        graphManager.AddNode(node3);
        graphManager.AddNode(node4);

        graphManager.AddBidirectionalConnections(node1, node2);
        graphManager.AddBidirectionalConnections(node2, node3);
        graphManager.AddBidirectionalConnections(node3, node4);

        SetPatrolRoute(patrolPoints);
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
            npcAnimator.SetBool("isInteract", true);
            OnPlayerEnter?.Invoke(true,"Herrera");
            npcData.movementForce = 0f;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            npcAnimator.SetBool("isInteract",false);
            OnPlayerEnter?.Invoke(false,"Herrera");
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

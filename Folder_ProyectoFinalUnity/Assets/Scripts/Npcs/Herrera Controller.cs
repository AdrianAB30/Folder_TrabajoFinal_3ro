using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class HerreraController : RoutePatrolDefined
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject[] nodes;

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
        SimpleLinkedList<GameObject> patrolPoints = new SimpleLinkedList<GameObject>();
        patrolPoints.InsertNodeAtEnd(nodes[0]);
        patrolPoints.InsertNodeAtEnd(nodes[1]);
        patrolPoints.InsertNodeAtEnd(nodes[2]);
        patrolPoints.InsertNodeAtEnd(nodes[3]);

        graphManager.AddNode(nodes[0]);
        graphManager.AddNode(nodes[1]);
        graphManager.AddNode(nodes[2]);
        graphManager.AddNode(nodes[3]);

        graphManager.AddDirectedConnection(nodes[0], nodes[1]);
        graphManager.AddDirectedConnection(nodes[1], nodes[2]);
        graphManager.AddDirectedConnection(nodes[2], nodes[3]);

        SetPatrolRoute(patrolPoints);
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
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueMark.SetActive(false);
            npcAnimator.SetBool("isInteract", true);
            OnPlayerEnter?.Invoke(true,"Herrera");
            movementForce = 0f;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            npcAnimator.SetBool("isInteract",false);
            OnPlayerEnter?.Invoke(false,"Herrera");
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
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }
    }
}

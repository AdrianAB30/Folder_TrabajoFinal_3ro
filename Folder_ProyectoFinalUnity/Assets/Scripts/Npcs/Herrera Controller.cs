using System.Collections;
using UnityEngine;

public class HerreraController : RoutePatrolDefined
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject node1;
    [SerializeField] private GameObject node2;
    [SerializeField] private GameObject node3;
    [SerializeField] private GameObject node4;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        SetNodesPatrol();
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
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class PatrolBehaviour : MonoBehaviour 
{
    protected SimpleLinkedList<GameObject> nodesRoutes;
    [SerializeField] protected int currentPatrolIndex;
    [SerializeField] protected float distanceToNode;
    [SerializeField] protected float movementForce;
    public bool isChasing = false;
    public Rigidbody myRBDRoute;
    public NavMeshAgent IA;
    public GameObject lastPatrolNode;

    protected virtual void Awake()
    {
        IA = GetComponent<NavMeshAgent>();
        myRBDRoute = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        currentPatrolIndex = 0;
    }

    protected virtual void FixedUpdate()
    {
        if (nodesRoutes != null && nodesRoutes.Length > 0)
        {
            Patrol();
        }
    }

    protected virtual void SetPatrolRoute(SimpleLinkedList<GameObject> patrolNodes)
    {
        nodesRoutes = patrolNodes;
        currentPatrolIndex = 0;
    }
    protected abstract void Patrol();

    protected void MoveTowards(Vector3 targetPosition)
    {
        if (myRBDRoute != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > distanceToNode)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;

                myRBDRoute.AddForce(direction * movementForce, ForceMode.Force);
            }
            else
            {
                myRBDRoute.velocity = Vector3.zero;
            }
        }
    }
}

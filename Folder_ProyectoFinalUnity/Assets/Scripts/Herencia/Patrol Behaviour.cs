using System;
using System.Collections;
using UnityEngine;

public abstract class PatrolBehaviour : MonoBehaviour 
{
    protected SimpleLinkedList<GameObject> nodesRoutes;
    [SerializeField] protected int currentPatrolIndex;
    [SerializeField] protected NPCData npcData;
    private Rigidbody myRBD;

    protected virtual void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
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

    public void SetPatrolRoute(SimpleLinkedList<GameObject> patrolNodes)
    {
        nodesRoutes = patrolNodes;
        currentPatrolIndex = 0;
    }
    protected abstract void Patrol();

    protected void MoveTowards(Vector3 targetPosition)
    {
        if (myRBD != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > npcData.distanceToNodeNPC)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                myRBD.AddForce(direction * npcData.movementForce * Time.deltaTime, ForceMode.Force);
            }
            else
            {
                myRBD.velocity = Vector3.zero;
            }
        }
    }

}

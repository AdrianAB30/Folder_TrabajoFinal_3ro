using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePatrolRandom : PatrolBehaviour
{
    private bool isWaiting = false; 
    public Animator npcRandomAnimator;

    protected override void Awake()
    {
        base.Awake();
        npcRandomAnimator = GetComponent<Animator>();
    }
    protected override void Start()
    {
        StartCoroutine(WaitAtNode());
        base.Start();
    }
    protected override void Patrol()
    {
        if (nodesRoutes.Length == 0 || isWaiting) 
        {
            return;
        }

        GameObject currentNode = nodesRoutes.GetNodeAtPosition(currentPatrolIndex);
        MoveTowards(currentNode.transform.position);
        LookAtNode(currentNode);

        if (Vector3.Distance(transform.position, currentNode.transform.position) > distanceToNode)
        {
            npcRandomAnimator.SetBool("isWalkingRandom", true);
        }
        else
        {
            npcRandomAnimator.SetBool("isWalkingRandom", false);
            StartCoroutine(WaitAtNode()); 
        }
    }
    private void LookAtNode(GameObject targetNode)
    {
        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * npcData.forceRotateNpc);
        }
    }
    private IEnumerator WaitAtNode()
    {
        isWaiting = true;
        npcRandomAnimator.SetBool("isWalkingRandom", false); 
        yield return new WaitForSeconds(npcData.watingTime); 

        currentPatrolIndex = Random.Range(0, nodesRoutes.Length);
        isWaiting = false;
    }
}

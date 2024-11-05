using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePatrolDefined : PatrolBehaviour
{
    private bool movingForward = true;
    private bool isWaiting = false;
    public Animator npcAnimator;

    protected override void Awake()
    {
        base.Awake();
        npcAnimator = GetComponent<Animator>(); 
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

        if (Vector3.Distance(transform.position, currentNode.transform.position) > npcData.distanceToNodeNPC)
        {
            npcAnimator.SetBool("isWalking", true); 
        }
        else
        {
            npcAnimator.SetBool("isWalking", false);
            StartCoroutine(WaitAtNode());
        }
    }
    private void LookAtNode(GameObject node)
    {
        Vector3 direction = (node.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); 
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * npcData.rotateNpc); 
    }
    private IEnumerator WaitAtNode()
    {
        isWaiting = true;
        npcAnimator.SetBool("isWalking", false);
        yield return new WaitForSeconds(npcData.watingTime);
        isWaiting = false;
        UpdatePatrolIndex(); 
    }
    private void UpdatePatrolIndex()
    {
        if (movingForward)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= nodesRoutes.Length)
            {
                currentPatrolIndex = nodesRoutes.Length - 1;
                movingForward = false;
            }
        }
        else
        {
            currentPatrolIndex--;
            if (currentPatrolIndex < 0)
            {
                currentPatrolIndex = 0;
                movingForward = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePatrolRandom : PatrolBehaviour
{
    public float waitTime;
    private bool isWaiting = false; 
    private Animator npcAnimator;

    protected virtual void Awake()
    {
        npcAnimator = GetComponent<Animator>();
    }
    protected override void Start()
    {
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

        if (Vector3.Distance(transform.position, currentNode.transform.position) > 0.1f)
        {
            npcAnimator.SetBool("isWalking", true);
        }
        else
        {
            npcAnimator.SetBool("isWalking", false);
            StartCoroutine(WaitAtNode()); 
        }
    }
    private void LookAtNode(GameObject targetNode)
    {
        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    private IEnumerator WaitAtNode()
    {
        isWaiting = true; 
        npcAnimator.SetBool("isWalking", false); 
        yield return new WaitForSeconds(waitTime); 

        currentPatrolIndex = Random.Range(0, nodesRoutes.Length);
        isWaiting = false;
    }
}

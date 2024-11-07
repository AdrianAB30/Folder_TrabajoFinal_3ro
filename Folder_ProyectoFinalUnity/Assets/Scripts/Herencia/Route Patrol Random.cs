using System.Collections;
using UnityEngine;

public class RoutePatrolRandom : PatrolBehaviour
{
    private bool isWaiting = false; 
    public Animator enemyAnimator;

    protected override void Awake()
    {
        base.Awake();
        enemyAnimator = GetComponent<Animator>();
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
            enemyAnimator.SetBool("isWalkingRandom", true);
        }
        else
        {
            enemyAnimator.SetBool("isWalkingRandom", false);
            StartCoroutine(WaitAtNode()); 
        }
    }
    private void LookAtNode(GameObject targetNode)
    {
        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * enemyData.forceRotateEnemy);
        }
    }
    private IEnumerator WaitAtNode()
    {
        isWaiting = true;
        enemyAnimator.SetBool("isWalkingRandom", false); 
        yield return new WaitForSeconds(enemyData.watingTime); 

        currentPatrolIndex = Random.Range(0, nodesRoutes.Length);
        isWaiting = false;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RoutePatrolRandom : PatrolBehaviour
{
    private bool isWaiting = false;
    public Animator enemyAnimator;
    public bool isTakingDamage;
    public bool isDead;
    public float forceRotateEnemy;

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
    // O(1)
    protected override void Patrol()
    {
        if (nodesRoutes.Length == 0 || isWaiting || isTakingDamage || isDead)
        {
            return;
        }

        GameObject currentNode = nodesRoutes.GetNodeAtPosition(currentPatrolIndex);
        MoveTowards(currentNode.transform.position);
        LookAtNode(currentNode);

        enemyAnimator.SetBool("isWalkingRandom", true);

        if (Vector3.Distance(transform.position, currentNode.transform.position) >= distanceToNode)
        {
            enemyAnimator.SetBool("isWalkingRandom", true);
        }
        else
        {
            enemyAnimator.SetBool("isWalkingRandom", false);
            if (!isWaiting) StartCoroutine(WaitAtNode());
        }
    }
    private void LookAtNode(GameObject targetNode)
    {
        if (isChasing || isTakingDamage || isDead) return;

        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * forceRotateEnemy);
        }
    }
    private IEnumerator WaitAtNode()
    {
        isWaiting = true;
        enemyAnimator.SetBool("isWalkingRandom", false);
        yield return new WaitForSeconds(1);

        currentPatrolIndex = Random.Range(0, nodesRoutes.Length);
        isWaiting = false;
    }
    public void CancelWaiting()
    {
        if (isWaiting)
        {
            StopCoroutine(WaitAtNode());
            isWaiting = false;
        }
    }
}
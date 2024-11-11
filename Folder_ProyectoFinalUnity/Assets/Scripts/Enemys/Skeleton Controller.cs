using System;
using System.Collections;
using UnityEngine;

public class SkeletonController : RoutePatrolRandom
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject skeleton;
    public int currentLife;
    public event Action<int> OnHealthChanged;


    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        currentLife = enemyData.maxLife;
        SetNodesPatrol();
    }
    private void SetNodesPatrol()
    {
        SimpleLinkedList<GameObject> pointsPatrol = new SimpleLinkedList<GameObject>();
        pointsPatrol.InsertNodeAtEnd(nodes[0]);
        pointsPatrol.InsertNodeAtEnd(nodes[1]);
        pointsPatrol.InsertNodeAtEnd(nodes[2]);
        pointsPatrol.InsertNodeAtEnd(nodes[3]);

        graphManager.AddNode(nodes[0]);
        graphManager.AddNode(nodes[1]);
        graphManager.AddNode(nodes[2]);
        graphManager.AddNode(nodes[3]);

        graphManager.AddBidirectionalConnections(nodes[0], nodes[1]);
        graphManager.AddBidirectionalConnections(nodes[1], nodes[2]);
        graphManager.AddBidirectionalConnections(nodes[2], nodes[3]);

        SetPatrolRoute(pointsPatrol);
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
    private void TakeDamage(int damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, enemyData.maxLife);
        Debug.Log("Invocando evento OnHealthChanged con currentLife: " + currentLife);
        OnHealthChanged?.Invoke(currentLife);

        if (currentLife <= 0)
        {
            KillEnemy();
        }
        else
        {
            myRBD.AddForce(enemyData.directionPushing * enemyData.pushingForceHit, ForceMode.Impulse);
            StartCoroutine(HitCoroutine());
        }
    }
    private IEnumerator HitCoroutine()
    {
        isTakingDamage = true;
        enemyAnimator.SetBool("isHit", true);

        yield return new WaitForSeconds(0.5f);

        isTakingDamage = false;
        enemyAnimator.SetBool("isHit", false);
    }
    private void KillEnemy()
    {
        isDead = true;
        enemyAnimator.SetTrigger("isDead");
        enemyAnimator.SetBool("isWalkingRandom",false);
        StartCoroutine(DeadEnemySkeleton());
        Debug.Log("Enemigo Muerto");
    }
    IEnumerator DeadEnemySkeleton()
    {
        yield return new WaitForSeconds(2f);
        skeleton.SetActive(false);
    }
    public int GetCurrentLife()
    {
        return currentLife;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            TakeDamage(20);
        }
    }
}
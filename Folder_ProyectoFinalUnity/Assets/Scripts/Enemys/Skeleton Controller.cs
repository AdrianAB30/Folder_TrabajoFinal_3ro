using System;
using System.Collections;
using UnityEngine;

public class SkeletonController : RoutePatrolRandom
{
    [Header("Patrol Data NPC")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject skeleton;
    [SerializeField] private GameObject objective;
    [SerializeField] private float detectionRadius = 5f;
    public int currentLife;

    //Eventos
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

        graphManager.AddDirectedConnection(nodes[0], nodes[1]);
        graphManager.AddDirectedConnection(nodes[1], nodes[2]);
        graphManager.AddDirectedConnection(nodes[2], nodes[3]);

        SetPatrolRoute(pointsPatrol);
    }
    private void TakeDamage(int damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, enemyData.maxLife);
        OnHealthChanged?.Invoke(currentLife);

        if (currentLife <= 0)
        {
            KillEnemy();
        }
        else
        {
            myRBDRoute.AddForce(enemyData.directionPushing * enemyData.pushingForceHit, ForceMode.Impulse);
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
        movementForce = 0f;
        StartCoroutine(DeadEnemySkeleton());
        Debug.Log("Enemigo Muerto");
    }
    private void ChasingPlayer()
    {
    }
    private void DetectPlayerInRange()
    {
        if (objective != null)
        {
            Vector3 playerPosition = objective.transform.position;
            Vector3 enemyPosition = transform.position;

            float distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);

            if (distanceToPlayer <= detectionRadius)
            {
                Debug.Log("Jugador detectado");
                ChasingPlayer(); 
            }
        }
    }
    IEnumerator DeadEnemySkeleton()
    {
        yield return new WaitForSeconds(2f);
        isDead = true;
        skeleton.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            isTakingDamage = true;
            TakeDamage(20);
        }
        else if (other.gameObject.CompareTag("Arrow"))
        {
            isTakingDamage = true;
            TakeDamage(15);
            Destroy(other.gameObject);
        }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

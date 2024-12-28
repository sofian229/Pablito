using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBreadAI : MonoBehaviour
{

    //Refrences
    public UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public Transform paltrollingPoints;
    public LayerMask Ground, Player, PaltrollingPoints;
    public int i = 1;

    //Patroling
    public Vector3 paltrollingPoint;
    bool paltrolling;
    public float sightRange;
    public bool playerInSightRange;
    public bool letEnemyMove;
    public float letEnemyMoveTimer;
    public float letEnemyMoveTimerReset = 0;

    //Next Point
    public int numberOfPoint;

    //Attacking
    public bool playerHit = false;
    public float attackTimer;

    private void Awake()
    {
        paltrollingPoints = GameObject.Find("PaltrollingPoints"+i).transform;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(i >= numberOfPoint)
        {
            i = 0;
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);

        if(playerInSightRange)
        {
           ChasePlayer();
        }
        else
        {
            Patroling();
        }
    }

    void Patroling()
    {
        agent.SetDestination(paltrollingPoints.position);
        Vector3 distanceToPatrolPoint = transform.position - paltrollingPoint;

        //Walkpoint reached
        if(distanceToPatrolPoint.magnitude < 1f)
        {
            i ++;
            paltrollingPoints = GameObject.Find("PaltrollingPoints"+i).transform;
        }

        paltrollingPoint = new Vector3 (paltrollingPoints.transform.position.x, paltrollingPoints.transform.position.y, paltrollingPoints.transform.position.z);
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        Vector3 distanceToPlayer = transform.position - paltrollingPoint;

        if(distanceToPlayer.magnitude < 1f)
        {
            playerHit = true;
            letEnemyMoveTimer = letEnemyMoveTimerReset;
        }

        if(playerHit)
        {
            //letEnemyMove = false;
            letEnemyMoveTimer += Time.deltaTime;
            if(letEnemyMoveTimer >= 2)
            {
                letEnemyMove = true;
                playerHit = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}

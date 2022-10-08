using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField]
    private Vector3 movePositionVector;

    private NavMeshAgent navMeshAgent;

    private bool canMove = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        movePositionVector = this.transform.position;
    }

    public void StartMove()
    {
        canMove = true;
        movePositionVector = GetRandomPositionNav();
    }

    // Update is called once per frame
    private void Update()
    {
        if (canMove) 
        {
            navMeshAgent.destination = movePositionVector;
        }
    }

    private Vector3 GetRandomPositionNav()
    {
        float walkRadius = 5f;

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }
}

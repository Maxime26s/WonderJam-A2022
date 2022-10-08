using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField]
    private Vector3 movePositionVector;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        movePositionVector = GetRandomPositionNav();
    }

    // Update is called once per frame
    private void Update()
    {
        navMeshAgent.destination = movePositionVector;
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

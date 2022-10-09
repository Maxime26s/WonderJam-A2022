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
        movePositionVector = this.transform.position;
    }

    public void StartMove()
    {
        movePositionVector = GetRandomPositionNav();

        StartCoroutine(Moving());
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

    IEnumerator Moving()
    {
        float timeLeft = 3.0f;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (navMeshAgent.isOnNavMesh)
                navMeshAgent.destination = movePositionVector;

            yield return null;
        }
        yield return null;
    }
}

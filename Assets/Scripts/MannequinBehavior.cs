using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class MannequinBehavior : MonoBehaviour
{
    private enum Behavior {
        Roaming,
        ChasingPlayer,
    }

    private Behavior behavior;

    private Animator _animator;
    private Rigidbody _rigidbody;

    private bool _tposing;
    private bool _walking;
    private bool _running;

    public bool spotted;
    private float spottedTimeLeft;

    [SerializeField] public float wanderRadius;
    [SerializeField] public float wanderTimer;
    private float roamTimer;
    private float tposeTimer;

    private NavMeshAgent _navMeshAgent;

    [SerializeField] public Transform player;
    [SerializeField] public int Health;
    private int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        hitCount = 0;
        spotted = false;

        tposeTimer = Random.Range(5, 10);
        spottedTimeLeft = 1.0f;
        roamTimer = wanderTimer;

        behavior = Behavior.Roaming;
    }

    // Update is called once per frame
    void Update()
    {
        roamTimer += Time.deltaTime;
        tposeTimer -= Time.deltaTime;
        spottedTimeLeft -= Time.deltaTime;

        if (!spotted)
        {
            if (tposeTimer <= 0)
            {
                tposeTimer = Random.Range(5, 10);
                StartCoroutine(TPosing());
            }

            switch (behavior)
            {
                case Behavior.Roaming:
                    Roam();
                    break;
                case Behavior.ChasingPlayer:
                    ChasePlayer();
                    break;
            }
        }
        else
        {
            _navMeshAgent.isStopped = true;
            _walking = false;
            _animator.SetBool("Walking", false);
            _running = false;
            _animator.SetBool("Running", false);
        }

        if (spottedTimeLeft <= 0)
        {
            spotted = false;
        }
    }

    private void ChasePlayer()
    {
        _animator.SetBool("Walking", false);
        _walking = false;
        _animator.SetBool("Running", true);
        _running = true;
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(player.transform.position);
    }

    private void Roam()
    {
        _animator.SetBool("Running", false);
        _running = false;
        _animator.SetBool("Walking", true);
        _walking = true;
        _navMeshAgent.isStopped = false;
        if (roamTimer >= wanderTimer)
        {
            Vector3 newRandomPos = GetRandomPos(transform.position, wanderRadius, -1);
            _navMeshAgent.SetDestination(newRandomPos);
            roamTimer = 0;
        }
    }

    IEnumerator TPosing()
    {
        float duration = Random.Range(1, 3);
        _tposing = true;
        _animator.SetBool("TPosing", true);

        while (duration>0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        _tposing = false;
        _animator.SetBool("TPosing", false);

        yield return null;
    }

    private Vector3 GetRandomPos(Vector3 origin, float dist, int layerMask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layerMask);
        return navHit.position;
    }

    public void Spotted()
    {
        //Debug.Log("Spotted "+ transform.name);
        spotted = true;
        spottedTimeLeft = 2.0f;
    }

    public void TakeDamage()
    {
        hitCount++;
        Health--;

        if (Health == 0)
        {
            Death();
        }

        if (hitCount == 1)
        {
            behavior = Behavior.ChasingPlayer;
        }
    }

    private void Death()
    {
        //Destroy(this);
    }
}

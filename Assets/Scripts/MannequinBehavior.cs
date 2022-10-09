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

    public bool spotted;
    private float spottedTimeLeft;

    [SerializeField] public float wanderRadius;
    [SerializeField] public float wanderTimer;
    private float roamTimer;
    private float tposeTimer;
    private float attackTimer;

    private NavMeshAgent _navMeshAgent;

    private Transform player;
    [SerializeField] public int Health;
    private int hitCount;

    [SerializeField] public Transform[] teleportPoints;
    private int nbTPPoints;

    [SerializeField] public float attackDuration = 1.5f;
    [SerializeField] public float attackSpinSpeed = 2000f;

    private bool justAttacked = false;
    private bool attacking = false;

    private float movementSpeed;
    [SerializeField] public float speedIncreasePerHit = 3f;

    private AudioSource _audioSource;
    [SerializeField] public AudioClip stepSound;
    [SerializeField] public AudioClip attackSound;
    [SerializeField] public float soundEffectsVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        hitCount = 0;
        spotted = false;

        tposeTimer = Random.Range(5, 10);
        spottedTimeLeft = 1.0f;
        roamTimer = wanderTimer;
        attackTimer = 5f;

        movementSpeed = _navMeshAgent.speed;

        behavior = Behavior.Roaming;
        nbTPPoints = teleportPoints.Length;
    }

    // Update is called once per frame
    void Update()
    {
        roamTimer += Time.deltaTime;
        tposeTimer -= Time.deltaTime;
        spottedTimeLeft -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        if (attacking)
        {
            SetAnim("TPosing", true);
            gameObject.transform.Rotate(0, attackSpinSpeed*Time.deltaTime, 0);
        }

        if (!spotted)
        {
            if (tposeTimer <= 0)
            {
                tposeTimer = Random.Range(5, 10);
                StartCoroutine(TPosing());
            }

            if (attackTimer < 0)
            {
                attackTimer = 5f;
                justAttacked = false;
            }

            switch (behavior)
            {
                case Behavior.Roaming:
                    _audioSource.volume = soundEffectsVolume / 3;
                    Roam();
                    break;
                case Behavior.ChasingPlayer:
                    _audioSource.volume = soundEffectsVolume;
                    ChasePlayer();
                    break;
            }
        }
        else
        {
            _navMeshAgent.isStopped = true;
            SetAnim("Walking", false);
            SetAnim("Running", false);
        }

        if (spottedTimeLeft <= 0)
        {
            spotted = false;
        }
    }

    private void ChasePlayer()
    {
        SetAnim("Walking", false);
        SetAnim("Running", true);
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(player.transform.position);

        float distToPlayer = Vector3.Distance(gameObject.transform.position, player.position);

        if (distToPlayer <= 2)
        {
            _navMeshAgent.isStopped = true;

            if (!justAttacked)
            {
                StartCoroutine(Attacking());
            }
        }
        else
        {
            _navMeshAgent.isStopped = false;
        }

    }

    private void Roam()
    {
        SetAnim("Running", false);
        SetAnim("Walking", true);
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
        SetAnim("TPosing", true);

        while (duration>0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        SetAnim("TPosing", false);
        yield return null;
    }

    IEnumerator Attacking()
    {
        _audioSource.PlayOneShot(attackSound);
        float attackDurationTimer = attackDuration;
        player.gameObject.GetComponent<PlayerController>().TakeDamage();
        attacking = true;
        justAttacked = true;


        while (attackDurationTimer > 0)
        {
            attackDurationTimer -= Time.deltaTime;
            yield return null;
        }

        attacking = false;
        SetAnim("TPosing", false);
        TeleportToRandomPoint();
        Debug.Log("Attacked player");
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

    private void TeleportToRandomPoint()
    {
        int randomTPPpoint = Random.Range(0, nbTPPoints);
        gameObject.transform.position = teleportPoints[randomTPPpoint].position;
    }

    private void SetAnim(string anim, bool state)
    {
        _animator.SetBool(anim, state);
    }

    public void Spotted()
    {
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
        } else
        {
            if (hitCount == 1)
            {
                behavior = Behavior.ChasingPlayer;
            }
            else
            {
                movementSpeed += speedIncreasePerHit;
                _navMeshAgent.speed = movementSpeed;
            }

            TeleportToRandomPoint();
        }
    }

    private void Step()
    {
        _audioSource.PlayOneShot(stepSound);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}

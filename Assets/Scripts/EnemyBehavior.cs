using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public enum GlitchType
    {
        Move = 0,
        Vibrate = 1,
        Stretch = 2,
        ChangeMaterial = 3,
        ChangeMeshError = 4,
        Fling = 5,
    }

    [SerializeField]
    public GlitchType glitchType = GlitchType.Vibrate;
    [SerializeField]
    float glitchTimerMin = 10.0f;
    [SerializeField]
    float glitchTimerMax = 20.0f;

    [SerializeField]
    GameObject player;

    float glitchTimer;

    AudioSource audioSource;

    [SerializeField] public AudioClip vibrateAudio;
    [SerializeField] public AudioClip glitchAudio;
    [SerializeField] public AudioClip wobbleAudio;
    [SerializeField] public AudioClip dragAudio;
    [SerializeField] public AudioClip flingAudio;

    EnemyNavMesh enemyNavMesh;
    Rigidbody rb;
    NavMeshAgent navMeshAgent;

    public bool spotted;

    public bool invincible;

    float spottedTimeLeft;
    private Renderer enemyRenderer;
    [SerializeField] public Material[] errorMaterials;
    private MeshFilter enemyMeshFilter;
    private Material[] defaultMaterials;

    public int health = 3;

    [SerializeField]
    private GameObject deathAnim;

    private bool dialoguePlayed = false;

    private void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        enemyMeshFilter = GetComponent<MeshFilter>();
        enemyNavMesh = GetComponent<EnemyNavMesh>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultMaterials = enemyRenderer.materials;

        SetTimer();

        spotted = false;
        spottedTimeLeft = 1.0f;
        invincible = false;
    }
    private void Update()
    {
        glitchTimer -= Time.deltaTime;
        spottedTimeLeft -= Time.deltaTime;

        if (glitchTimer <= 0)
        {
            SetTimer();
            Glitch();
        }
        if (spottedTimeLeft <= 0)
        {
            spotted = false;
        }

    }
    private void SetTimer()
    {
        glitchTimer = Random.Range(glitchTimerMin, glitchTimerMax);
    }

    private void Glitch()
    {
        if (!spotted)
        {
            ExecuteGlitch();
        }
    }

    private void ExecuteGlitch()
    {
        switch (glitchType)
        {
            case GlitchType.Move:           //move to random navmesh location
                audioSource.clip = dragAudio;
                move();
                break;
            case GlitchType.Vibrate:        //vibrate for x seconds
                audioSource.clip = vibrateAudio;
                vibrate();
                break;
            case GlitchType.Stretch:
                audioSource.clip = wobbleAudio;
                stretch();
                break;
            case GlitchType.ChangeMaterial:
                audioSource.clip = glitchAudio;
                ChangeMaterial();
                break;
            case GlitchType.ChangeMeshError:
                audioSource.clip = glitchAudio;
                ChangeMeshError();
                break;
            case GlitchType.Fling:
                audioSource.clip = flingAudio;
                Fling();
                break;
        }
    }

    private void move()
    {
        enemyNavMesh.StartMove();
    }
    private void vibrate()
    {
        StartCoroutine(Vibration());
    }
    private void stretch()
    {
        StartCoroutine(Stretching());
    }

    private void ChangeMaterial()
    {
        StartCoroutine(ChangingMaterial());
    }
    private void Fling()
    {
        StartCoroutine(Flinging());
    }

    private void ChangeMeshError()
    {
        StartCoroutine(ChangingMesh());
    }

    IEnumerator Vibration()
    {
        audioSource.Play(0);

        float speed = 10.0f;
        float intensity = 0.5f;

        float timeLeft = 3.0f;

        Vector3 startTransform = gameObject.transform.position;

        Debug.Log(startTransform.x +", " + startTransform.y + ", "+ startTransform.z);


        //GetComponent<Rigidbody>().isKinematic = false;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            transform.position =  new Vector3(
                startTransform.x + (intensity * Mathf.PerlinNoise(speed * Time.time, 1)),
                startTransform.y + (intensity * Mathf.PerlinNoise(speed * Time.time, 2)),
                startTransform.z + (intensity * Mathf.PerlinNoise(speed * Time.time, 3)));

            yield return null;
        }
        transform.position = startTransform;

        //GetComponent<Rigidbody>().isKinematic = true;


        audioSource.Stop();
        yield return null;
    }

    IEnumerator Stretching()
    {
        audioSource.Play(0);

        float speed = 1.0f;
        float intensity = 0.5f;

        float timeLeft = 4.0f;

        Vector3 startScale = transform.localScale;
        transform.localScale = startScale;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            transform.localScale = new Vector3(
                startScale.x + (intensity * Mathf.PerlinNoise(speed * Time.time, 1)),
                startScale.y + (intensity * Mathf.PerlinNoise(speed * Time.time, 2)),
                startScale.z + (intensity * Mathf.PerlinNoise(speed * Time.time, 3)));
            yield return null;
        }
        transform.localScale = startScale;

        audioSource.Stop();
        yield return null;
    }

    public void Spotted()
    {
        spotted = true;

        spottedTimeLeft = 2.0f;
    }

    public void ForceGlitch()
    {
        ExecuteGlitch();
    }

    IEnumerator ChangingMaterial()
    {
        audioSource.Play(0);

        if (enemyRenderer != null)
        {
            var oldMaterials = enemyRenderer.materials;
            var randomTime = Random.Range(1.0f, 4.0f);
            var randomMatIndex = Random.Range(0, errorMaterials.Length);
            Material errorMaterial = errorMaterials[randomMatIndex];
            Material[] newMaterials = new Material[1];
            newMaterials[0] = errorMaterial;

            enemyRenderer.materials = newMaterials;

            yield return new WaitForSeconds(randomTime);

            enemyRenderer.materials = defaultMaterials;
        }
        audioSource.Stop();
        yield return null;
    }

    IEnumerator ChangingMesh()
    {
        audioSource.Play(0);

        if (enemyRenderer != null)
        {
            var randomTime = Random.Range(1.0f, 4.0f);
            Mesh oldMesh = enemyMeshFilter.mesh;
            Material oldMaterial = enemyRenderer.material;
            enemyMeshFilter.mesh = Resources.Load<Mesh>("error");
            enemyRenderer.material = Resources.Load<Material>("error");
            yield return new WaitForSeconds(randomTime);
            enemyMeshFilter.mesh = oldMesh;
            enemyRenderer.material = oldMaterial;
        }
        audioSource.Stop();
        yield return null;
    }
    IEnumerator Flinging()
    {
        if (flingAudio)
        audioSource.PlayOneShot(flingAudio);

        float maxForce = 1.0f;
        float intensity = 400.0f;

        float timeLeft = 1.2f;

        Vector3 flingDirection = new Vector3(Random.Range(-maxForce, maxForce), Random.Range(0, maxForce), Random.Range(-maxForce, maxForce));
        flingDirection.Normalize();
        rb.isKinematic = false;
        navMeshAgent.enabled = false;

        rb.AddForce(flingDirection * intensity);

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        rb.isKinematic = true;
        navMeshAgent.enabled = true;

        //transform.position = FindNearestNavMeshPoint(transform.position);
        //audioSource.Stop();
        yield return null;
    }

    public void TakeDamage()
    {
        if (!dialoguePlayed)
            dialoguePlayed = DialogueDatabase.Instance.TryPlayEnemyDialogue(gameObject.name);

        if (!invincible)
        {
            health--;
            if (health <= 0)
            {
                Death();
            }
            else
            {
                Flee();
            }
        }
    }

    private void Flee()
    {
        float walkRadius = 10f;

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;

        StartCoroutine(Fleeing(finalPosition));
        StartCoroutine(Immunity());
    }

    IEnumerator Fleeing(Vector3 movePositionVector)
    {
        float timeLeft = 2.0f;


        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (navMeshAgent.isOnNavMesh)
            navMeshAgent.destination = movePositionVector;

            yield return null;
        }

        yield return null;
    }
    IEnumerator Immunity()
    {
        float timeLeft = 0.5f;

        invincible = true;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            yield return null;
        }
        invincible = false;

        yield return null;
    }

    public void Death()
    {
        this.transform.parent.GetComponent<EnemyManager>().selectedEnemies.Remove(gameObject);
        Destroy(gameObject);
        GameManager.Instance.IsLevelEnd();
    }
}

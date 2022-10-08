using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private enum GlitchType {
        Move = 0,
        Vibrate = 1,
        Stretch = 2,
        ChangeMaterial = 3,
        ChangeMeshError = 4,
        Fling = 5,
    }

    [SerializeField]
    GlitchType glitchType = GlitchType.Vibrate;
    [SerializeField]
    float glitchTimerMin = 10.0f;
    [SerializeField]
    float glitchTimerMax = 20.0f;

    [SerializeField]
    GameObject player;

    float glitchTimer;

    AudioSource audioSource;

    EnemyNavMesh enemyNavMesh;

    Rigidbody rb;

    NavMeshAgent navMeshAgent;

    public bool spotted;

    float spottedTimeLeft;
    private Renderer enemyRenderer;
    [SerializeField] public Material[] errorMaterials;
    private MeshFilter enemyMeshFilter;
    private Material[] defaultMaterials;

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
        if(spottedTimeLeft <= 0)
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
            switch (glitchType)
            {
                case GlitchType.Move:         //move to random navmesh location
                    move();
                    break;
                case GlitchType.Vibrate:         //vibrate for x seconds
                    vibrate();
                    break;
                case GlitchType.Stretch:
                    stretch();
                    break;
                case GlitchType.ChangeMaterial:
                    ChangeMaterial();
                    break;
                case GlitchType.Fling:
                    Fling();
                    break;
                    
            }
        }
        
        RaycastHit hit;
        Vector3 rayDirection = player.transform.position - transform.position;
        rayDirection.Normalize();

        if(Physics.Raycast (transform.position, rayDirection, out hit))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform != player.transform)
            {
                switch (glitchType)
                {
                    case GlitchType.Move:         //move to random navmesh location
                        move();
                        break;
                    case GlitchType.Vibrate:         //vibrate for x seconds
                        vibrate();
                        break;
                    case GlitchType.Stretch:
                        stretch();
                        break;
                    case GlitchType.ChangeMaterial:
                        ChangeMaterial();
                        break;
                    case GlitchType.ChangeMeshError:
                        ChangeMeshError();
                        break;
                    case GlitchType.Fling:
                        Fling();
                        break;
                }
            }
            else 
            {
                Debug.Log("you can see the guy");
            }
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
        float intensity = 0.1f;

        float timeLeft = 3.0f;

        Vector3 startTransform = transform.position;
        transform.position = startTransform;

        while (timeLeft>0)
        {
            timeLeft -= Time.deltaTime;

            transform.localPosition =  new Vector3(
                startTransform.x + (intensity * Mathf.PerlinNoise(speed * Time.time, 1)),
                startTransform.y + (intensity * Mathf.PerlinNoise(speed * Time.time, 2)),
                startTransform.z + (intensity * Mathf.PerlinNoise(speed * Time.time, 3)));
            yield return null;
        }
        transform.position = startTransform;


        audioSource.Stop();
        yield return null;
    }

    IEnumerator Stretching()
    {
        float speed = 1.0f;
        float intensity = 0.5f;

        float timeLeft = 4.0f;

        Vector3 startScale = transform.localScale;
        transform.localScale = startScale;

        while(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            transform.localScale = new Vector3(
                startScale.x + (intensity * Mathf.PerlinNoise(speed * Time.time, 1)),
                startScale.y + (intensity * Mathf.PerlinNoise(speed * Time.time, 2)),
                startScale.z + (intensity * Mathf.PerlinNoise(speed * Time.time, 3)));
            yield return null;
        }
        transform.localScale = startScale;

        yield return null;
    }

    public void Spotted()
    {
        Debug.Log("Spotted "+ transform.name);

        spotted = true;

        spottedTimeLeft = 2.0f;
    }

    IEnumerator ChangingMaterial()
    {
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
    }

    IEnumerator ChangingMesh()
    {
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
    }
    IEnumerator Flinging()
    {
        float maxForce = 10.0f;
        float intensity = 500.0f;

        float timeLeft = 2.0f;

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
        yield return null;
    }
}

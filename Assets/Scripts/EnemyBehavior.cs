using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private enum GlitchType {
        Move = 0,
        Vibrate = 1,
        Stretch = 2,
        ChangeColor = 3,
        ChangeMaterial = 4,
        Fling = 6,
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

    private void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        enemyNavMesh = GetComponent<EnemyNavMesh>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

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
                case GlitchType.ChangeColor:
                    ChangeColor();
                    break;
                case GlitchType.ChangeMaterial:
                    ChangeMaterial();
                    break;
                case GlitchType.Fling:
                    Fling();
                    break;
                    
            }
        }
        /*
        RaycastHit hit;
        Vector3 rayDirection = player.transform.position - transform.position;
        rayDirection.Normalize();

        if(Physics.Raycast (transform.position, rayDirection, out hit))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform != player.transform)
            {
                
            }
            else 
            {
                Debug.Log("you can see the guy");
            }
        }*/
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
    private void ChangeColor()
    {
        StartCoroutine(ChangingColor());
    }
    private void ChangeMaterial()
    {
        StartCoroutine(ChangingMaterial());
    }
    private void Fling()
    {
        StartCoroutine(Flinging());
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

    IEnumerator ChangingColor()
    {
        float timeLeft = 4.0f;

        Color startingColor = enemyRenderer.material.GetColor("_Color");
        enemyRenderer.material.SetColor("_Color", startingColor);

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            Color newColor = Random.ColorHSV();
            enemyRenderer.material.SetColor("_Color", new Color());
            yield return null;
        }

        enemyRenderer.material.SetColor("_Color", Random.ColorHSV());
        yield return null;
    }

    IEnumerator ChangingMaterial()
    {
        if (enemyRenderer != null)
        {
            var randomTime = Random.Range(1.0f, 4.0f);
            var randomMatIndex = Random.Range(0, errorMaterials.Length);
            Material oldMaterial = enemyRenderer.material;
            enemyRenderer.material = errorMaterials[randomMatIndex];
            yield return new WaitForSeconds(randomTime);
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
    /*          //only finds vertical points
    private Vector3 FindNearestNavMeshPoint(Vector3 sourcePosition)
    {
        Vector3 nearest = new Vector3(0,0,0);
        NavMeshHit hit;
        float maxDistance = 5000f;

        NavMesh.SamplePosition(sourcePosition, out hit, maxDistance,0);

        Debug.Log(hit.position.x);
        nearest = hit.position;


        return nearest;
    }*/
}

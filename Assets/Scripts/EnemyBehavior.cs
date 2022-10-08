using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private enum GlitchType {
        Move = 0,
        Vibrate = 1,
        Stretch = 2,
        ChangeColor = 3,
    }

    [SerializeField]
    GlitchType glitchType = GlitchType.Vibrate;

    [SerializeField]
    float glitchTimerMin = 10.0f;
    [SerializeField]
    float glitchTimerMax = 20.0f;

    float glitchTimer;

    AudioSource audioSource;

    EnemyNavMesh enemyNavMesh;

    private void Start()
    {
        enemyNavMesh = GetComponent<EnemyNavMesh>();
        audioSource = GetComponent<AudioSource>();
        SetTimer();
    }
    private void Update()
    {
        glitchTimer -= Time.deltaTime;
        if (glitchTimer <= 0)
        {
            SetTimer();
            Glitch();
        } 
    }
    private void SetTimer()
    {
        glitchTimer = Random.Range(glitchTimerMin, glitchTimerMax);
    }

    private void Glitch()
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

    private void ChangeColor()
    {

    }

    IEnumerator Vibration()
    {
        float speed = 10.0f;
        float intensity = 0.1f;

        float timeLeft = 5.0f;

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
/*
    IEnumerator Rotating()
    {
        float speed = 1.0f;
        float intensity = 0.5f;

        float timeLeft = 4.0f;

        Vector3 startRotation = transform.localRotation;
        transform.rotation = startRotation;

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

        yield return null;
    }*/
}

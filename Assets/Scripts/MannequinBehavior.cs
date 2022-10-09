using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class MannequinBehavior : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody _rigidbody;

    private bool _tposing;
    private bool _walking;

    public bool spotted;
    private float spottedTimeLeft;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        spotted = false;
        spottedTimeLeft = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        spottedTimeLeft -= Time.deltaTime;

        if (spottedTimeLeft <= 0)
        {
            spotted = false;
        }
    }

    public void Spotted()
    {
        Debug.Log("Spotted "+ transform.name);
        spotted = true;
        spottedTimeLeft = 2.0f;

    }
}

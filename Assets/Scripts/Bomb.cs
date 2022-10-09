using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [SerializeField]
    public Animation bombAnimation = null;
    [SerializeField]
    private float explosionRadius = 10f;
    [SerializeField]
    private int fuseDuration = 3;
    [SerializeField]
    private float timeLeft = 0f;
    [SerializeField]
    private float forwardForce = 500f;
    [SerializeField]
    public bool IsThrown = false;
    [SerializeField]
    private GameObject explosion;

    public bool onCooldown = false;

    private Rigidbody rb;
    private SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
    }

    public void StartFuse()
    {
        timeLeft = fuseDuration;
    }

    private void Explode()
    {
        Destroy(gameObject);
        GameObject newExplosion = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        newExplosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        Destroy(newExplosion, newExplosion.GetComponent<ParticleSystem>().main.duration);

        Collider[] hitColliders = Physics.OverlapSphere(newExplosion.transform.GetComponent<Renderer>().bounds.center, explosionRadius);
        foreach(Collider hitCollider in hitColliders)
        {
            EnemyBehavior enemyBehavior = hitCollider.gameObject.GetComponent<EnemyBehavior>();
            if(enemyBehavior != null)
            {
                enemyBehavior.ForceGlitch();
            }
        }
    }

    private void Throw()
    {
            Debug.Log("Thrown");
            IsThrown = true;
            sphereCollider.enabled = true;
            gameObject.transform.parent = null;
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * forwardForce);
            StartFuse();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsThrown)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
                Explode();
        }

        if(InputManager.Instance.PlayerGetFireInput() && !onCooldown)
        {
            if(!IsThrown)
                Throw();
        }
    }
}

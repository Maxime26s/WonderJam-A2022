using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 10f;
    [SerializeField]
    private int fuseDuration = 3;
    [SerializeField]
    private float timeLeft = 0f;
    [SerializeField]
    private float forwardForce = 500f;
    [SerializeField]
    private bool IsThrown = false;
    [SerializeField]
    private GameObject explosion;

    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        parent = gameObject.transform.parent.gameObject;
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

        if(InputManager.Instance.PlayerGetFireInput())
        {
            if(!IsThrown)
                Throw();
        }
    }
}

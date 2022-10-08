using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject shellPrefab = null;
    [SerializeField]
    private Transform ejectDirection = null;
    [SerializeField]
    private float ejectForce = 5f;

    public void SpawnShell() {
        GameObject shellInstance = Instantiate(shellPrefab, transform.position, Quaternion.identity);
        Destroy(shellInstance, 5f);
        shellInstance.GetComponent<Rigidbody>().AddForce(ejectDirection.localPosition * ejectForce);
    }
}

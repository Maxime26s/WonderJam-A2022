using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShotgunUIEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject floatingShell;
    [SerializeField]
    private GameObject fallingShell;
    [SerializeField]
    private GameObject shotgun;
    [SerializeField]
    private int nbBullets = 5;
    [SerializeField]
    private float distanceBetweenShells = 1f;
    [SerializeField]
    private float startSpawnX = -8f;
    [SerializeField]
    private float startSpawnY = 4f;

    private List<GameObject> shells = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        UnityEvent shootGun = shotgun.GetComponent<Shotgun>().shootGun;
        shootGun.AddListener(DropShell);
        SpawnBullets();
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < nbBullets; i++)
        {
            GameObject newBullet = Instantiate(floatingShell, new Vector3(startSpawnX + i * distanceBetweenShells, startSpawnY, 0), Quaternion.identity);
            Vector3 newRotation = new Vector3(135, 0, 0);
            newBullet.transform.eulerAngles = newRotation;

            shells.Add(newBullet);
        }
    }

    private void DropShell()
    {
        int number = shells.Count;
        GameObject shell = shells[number - 1];
        Instantiate(fallingShell, shell.transform.position, shell.transform.rotation);
        Destroy(shell);
        shells.RemoveAt(number - 1);

        if (number - 1 < 1)
            SpawnBullets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

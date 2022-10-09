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
    private GameObject player;
    [SerializeField]
    private Transform boxTransform;
    [SerializeField]
    private int nbBullets = 5;
    [SerializeField]
    private float distanceBetweenShells = 1f;
    [SerializeField]
    private float startSpawnX = -7f;
    [SerializeField]
    private float startSpawnY = 4f;
    [SerializeField]
    private float playerJumpForce = 1f;

    private List<GameObject> shells = new List<GameObject>();
    private List<GameObject> droppedShells = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        UnityEvent shootGun = shotgun.GetComponent<Shotgun>().shootGun;
        shootGun.AddListener(DropShell);
        player.GetComponent<PlayerController>().playerJumping.AddListener(ShellJump);
        SpawnBullets();
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < nbBullets; i++)
        {
            GameObject newBullet = Instantiate(floatingShell, new Vector3(boxTransform.position.x + startSpawnX + i * distanceBetweenShells, boxTransform.position.y + startSpawnY, boxTransform.position.z), Quaternion.identity);
            Vector3 newRotation = new Vector3(135, 0, 0);
            newBullet.transform.eulerAngles = newRotation;

            shells.Add(newBullet);
        }
    }

    private void DropShell()
    {
        int number = shells.Count;
        GameObject shell = shells[number - 1];
        GameObject droppingShell = Instantiate(fallingShell, shell.transform.position, shell.transform.rotation);
        droppedShells.Add(droppingShell);
        Destroy(shell);
        shells.RemoveAt(number - 1);

        if (number - 1 < 1)
            SpawnBullets();
    }

    private void ShellJump()
    {
        int number = droppedShells.Count;
        for (int i = 0; i < number; i++)
        {
            GameObject droppedShell = droppedShells[i];

            droppedShell.GetComponent<Rigidbody>().AddForce(transform.up * playerJumpForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

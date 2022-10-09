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
    private GameObject floatingHeart;
    [SerializeField]
    private GameObject fallingHeart;
    [SerializeField]
    private GameObject shotgun;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Transform boxTransform;
    [SerializeField]
    private int nbBullets = 5;
    [SerializeField]
    private int nbHearts = 5;
    [SerializeField]
    private float distanceBetweenShells = 1f;
    [SerializeField]
    private float distanceBetweenHearts = 1.3f;
    [SerializeField]
    private float shellStartSpawnX = -7f;
    [SerializeField]
    private float shellStartSpawnY = 4f;
    [SerializeField]
    private float heartStartSpawnX = 5f;
    [SerializeField]
    private float heartStartSpawnY = 3.5f;
    [SerializeField]
    private float playerJumpForce = 1f;

    private List<GameObject> shells = new List<GameObject>();
    private List<GameObject> hearts = new List<GameObject>();
    private List<GameObject> droppedShells = new List<GameObject>();
    private List<GameObject> droppedHearts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        shotgun.GetComponent<Shotgun>().shootGun.AddListener(DropShell);
        player.GetComponent<PlayerController>().playerJumping.AddListener(ObjectsJump);
        player.GetComponent<PlayerController>().playerTakeDamage.AddListener(DropHeart);
        nbHearts = player.GetComponent<PlayerController>().health;
        SpawnBullets();
        SpawnHearts();
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < nbBullets; i++)
        {
            GameObject newBullet = Instantiate(floatingShell, new Vector3(boxTransform.position.x + shellStartSpawnX + i * distanceBetweenShells, boxTransform.position.y + shellStartSpawnY, boxTransform.position.z), Quaternion.identity);
            Vector3 newRotation = new Vector3(135, 0, 0);
            newBullet.transform.eulerAngles = newRotation;

            shells.Add(newBullet);
        }
    }

    private void SpawnHearts()
    {
        for (int i = 0; i < nbHearts; i++)
        {
            GameObject newHeart = Instantiate(floatingHeart, new Vector3(boxTransform.position.x + heartStartSpawnX + i * distanceBetweenHearts, boxTransform.position.y + heartStartSpawnY, boxTransform.position.z), Quaternion.identity);
            Vector3 newRotation = new Vector3(0, 90, 0);
            newHeart.transform.eulerAngles = newRotation;

            hearts.Add(newHeart);
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

    private void DropHeart()
    {
        if (hearts.Count <= 0)
            return;

        GameObject heart = hearts[0];
        GameObject droppingHeart = Instantiate(fallingHeart, heart.transform.position, heart.transform.rotation);
        droppedHearts.Add(droppingHeart);
        Destroy(heart);
        hearts.RemoveAt(0);
    }

    private void ObjectsJump()
    {
        int shellNumber = droppedShells.Count;
        for (int i = 0; i < shellNumber; i++)
        {
            GameObject droppedShell = droppedShells[i];

            droppedShell.GetComponent<Rigidbody>().AddForce(transform.up * playerJumpForce);
        }

        int heartNumber = droppedHearts.Count;
        for (int i = 0; i < heartNumber; i++)
        {
            GameObject droppedHeart = droppedHearts[i];

            droppedHeart.GetComponent<Rigidbody>().AddForce(transform.up * playerJumpForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

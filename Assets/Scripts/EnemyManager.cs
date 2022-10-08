using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> potentialEnemies = new List<GameObject>();
    public List<GameObject> selectedEnemies = new List<GameObject>();

    [SerializeField]
    int numberOfEnemies;

    private void Start()
    {
        foreach(Transform child in transform)
        {
            potentialEnemies.Add(child.gameObject);
        }
        
        for(int i = 0; i < numberOfEnemies; i++)
        {
            int selected = 0;

            selected = Random.Range(0, potentialEnemies.Count);

            selectedEnemies.Add(potentialEnemies[selected]);

            potentialEnemies.RemoveAt(selected);
        }
        foreach(GameObject enemy in selectedEnemies)
        {
            enemy.AddComponent(typeof(Rigidbody));
            enemy.AddComponent(typeof(AudioSource));
            enemy.AddComponent(typeof(EnemyBehavior));
            enemy.AddComponent(typeof(EnemyNavMesh));
            enemy.AddComponent(typeof(NavMeshAgent));

            int glitchType = Random.Range(0, 5); //TODO make number of element in enum dynamic

            enemy.GetComponent<EnemyBehavior>().glitchType = (EnemyBehavior.GlitchType)glitchType;

            enemy.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}

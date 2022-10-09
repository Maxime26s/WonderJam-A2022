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

    [SerializeField]
    public Material[] errorMaterialsManager;

    [SerializeField] public AudioClip vibrateAudio;
    [SerializeField] public AudioClip glitchAudio;
    [SerializeField] public AudioClip wobbleAudio;
    [SerializeField] public AudioClip dragAudio;

    private void Start()
    {
        foreach(Transform child in transform)
        {
            potentialEnemies.Add(child.gameObject);

            child.tag = "Prop";
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
            enemy.tag = "Enemy";

            enemy.AddComponent(typeof(Rigidbody));
            enemy.AddComponent(typeof(AudioSource));
            enemy.AddComponent(typeof(NavMeshAgent));
            enemy.AddComponent(typeof(EnemyBehavior));
            enemy.AddComponent(typeof(EnemyNavMesh));

            int glitchType = Random.Range(0, 6); //TODO make number of element in enum dynamic

            enemy.GetComponent<EnemyBehavior>().glitchType = (EnemyBehavior.GlitchType)glitchType;

            enemy.GetComponent<EnemyBehavior>().errorMaterials = errorMaterialsManager;

            enemy.GetComponent<NavMeshAgent>().baseOffset = 0.05f;

            enemy.GetComponent<EnemyBehavior>().vibrateAudio = vibrateAudio;
            enemy.GetComponent<EnemyBehavior>().wobbleAudio = wobbleAudio;
            enemy.GetComponent<EnemyBehavior>().glitchAudio = glitchAudio;
            enemy.GetComponent<EnemyBehavior>().dragAudio = dragAudio;

            enemy.GetComponent<AudioSource>().volume = 0.5f;
            enemy.GetComponent<AudioSource>().spatialBlend = 1;
            enemy.GetComponent<AudioSource>().playOnAwake = false;
            enemy.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
            enemy.GetComponent<AudioSource>().minDistance = 1;
            enemy.GetComponent<AudioSource>().maxDistance = 10;

            enemy.GetComponent<Rigidbody>().isKinematic = true;


            GameManager.Instance.SetTotalEnemyCount();

        }
    }
}

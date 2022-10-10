using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMenu : MonoBehaviour
{
    void Start()
    {
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(20f);
            SceneManager.LoadScene("MainMenu");
        }

        StartCoroutine(Delay());
    }
}

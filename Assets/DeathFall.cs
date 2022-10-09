using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathFall : MonoBehaviour
{
    public Fade fade;

    public TextMeshProUGUI titleObject;
    public TextMeshProUGUI loreObject;
    public string title;
    public string lore;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Player" || loreObject.color.a != 0)
            return;

        titleObject.text = title;
        loreObject.text = lore;

        fade.gameObject.SetActive(true);
        fade.StartFading();
    }
}

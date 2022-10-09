using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorRotation : MonoBehaviour
{
    public Image image;
    public float rotationSpeed = 0.125f;
    public float hue = 0.9f;
    public float saturation = 0.85f;
    public float value = 0.5f;

    // Update is called once per frame
    void Update()
    {
        hue += Time.deltaTime * rotationSpeed;
        if (hue > 1)
            hue = 0;
        image.color = Color.HSVToRGB(hue, saturation, value);
    }
}

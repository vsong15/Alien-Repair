using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningIndicator : MonoBehaviour
{
    public Image image;
    public Slider slider;
    public float timeOn = 0.5f;
    public float timeOff = 0.25f;
    public float warningValue = 0.25f;
    float timer;

    // Update is called once per frame
    void Update()
    {
        if (slider.value <= warningValue) {
            timer += Time.deltaTime;
            if (image.enabled && timer >= timeOn) {
                image.enabled = false;
                timer = 0;
            }
            if (!image.enabled && timer >= timeOff) {
                image.enabled = true;
                timer = 0;
            }
        } else {
            timer = 0;
            image.enabled = false;
        }
    }
}

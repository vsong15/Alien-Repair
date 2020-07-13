using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    public bool flash;
    float timer;
    bool on;
    public SpriteRenderer[] spriteRenderers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flash) {
            timer += Time.deltaTime;
            if (timer >= GameConfig.instance.flashIntervalSeconds) {
                timer -= GameConfig.instance.flashIntervalSeconds;
                on = !on;
            }
            foreach(var s in spriteRenderers)
                s.material.SetFloat("_Flash", on ? 1 : 0);
        } else {
            timer = 0;
            foreach(var s in spriteRenderers)
                s.material.SetFloat("_Flash", 0);
        }
    }
}

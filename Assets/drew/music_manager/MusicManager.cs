using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip titleMusic;
    [SerializeField]
    private AudioClip ambientMusic;
    [SerializeField]
    private List<AudioClip> ambientSFX;
    [SerializeField]
    private AudioSource musicPlayer;
    [SerializeField]
    private AudioSource sfxPlayer;

    private MusicManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        musicPlayer.clip = titleMusic;
        musicPlayer.Play();
    }

    public void PlayAmbientMusic()
    {
        musicPlayer.clip = ambientMusic;
        musicPlayer.Play();
        StartCoroutine(AmbientSoundRoutine());
    }

    private IEnumerator AmbientSoundRoutine()
    {
        while (true)
        {
            var wait = Random.Range(5, 10);
            yield return new WaitForSeconds(wait);
            var index = Random.Range(0, ambientSFX.Count);
            sfxPlayer.clip = ambientSFX[index];
            sfxPlayer.Play();
        }
    }
}

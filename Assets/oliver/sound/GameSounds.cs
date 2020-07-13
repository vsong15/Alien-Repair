using UnityEngine;
using System.Collections.Generic;

public class GameSounds : MonoBehaviour {

    public static GameSounds instance;

    [SerializeField]
    private List<AudioClip> footstepsDry;
    [SerializeField]
    private List<AudioClip> footstepsWet;
    [SerializeField]
    private AudioClip uiConfirm;
    [SerializeField]
    private AudioClip uiSelect;
    [SerializeField]
    private List<AudioClip> pipeBurst;
    [SerializeField]
    private List<AudioClip> deathDrown;
    [SerializeField]
    private List<AudioClip> ladderClimb;
    [SerializeField]
    private AudioClip gameWin;
    [SerializeField]
    private AudioClip gameLose;
    [SerializeField]
    private AudioClip startCrash;
    [SerializeField]
    private List<AudioClip> doorBurst;
    [SerializeField]
    private List<AudioClip> doorClose;
    [SerializeField]
    private List<AudioClip> leverLatch;

    private List<AudioSource> sources;
    public AudioSource sourcePrefab;
    
    void Awake() {
        sources = new List<AudioSource>();
        instance = this;
    }
    
    public void PlayClip(AudioClip clip, float volume = 1) {
        AudioSource source = null;
        for (int i = 0; i < sources.Count; i++) {
            if (!sources[i].isPlaying) {
                source = sources[i];
                break;
            }
        }
        if (!source) {
            source = Instantiate(sourcePrefab);
            source.transform.SetParent(transform);
            source.playOnAwake = false;
            sources.Add(source);
        }
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }


    public void PlayRandomClip(List<AudioClip> clipPool, float volume = 1)
    {
        var index = Random.Range(0, clipPool.Count);
        var clip = clipPool[index];
        PlayClip(clip, volume);
    }

    public void PlayerFootstepsDry()
    {
        PlayRandomClip(footstepsWet);
    }

    public void PlayerFootstepsWet()
    {
        PlayRandomClip(footstepsDry);
    }

    public void UIConfirm()
    {
        PlayClip(uiConfirm);
    }

    public void UISelect()
    {
        PlayClip(uiSelect);
    }

    public void PipeBurst()
    {
        PlayRandomClip(pipeBurst);
    }

    public void DeathDrown()
    {
        PlayRandomClip(deathDrown);
    }

    public void LadderClimb()
    {
        PlayRandomClip(ladderClimb);
    }

    public void GameWin()
    {
        PlayClip(gameWin);
    }

    public void GameLose()
    {
        PlayClip(gameLose);
    }

    public void StartCrash()
    {
        PlayClip(startCrash);
    }

    public void DoorBurst()
    {
        PlayRandomClip(doorBurst);
    }

    public void DoorClose()
    {
        PlayRandomClip(doorClose);
    }

    public void LeverLatch()
    {
        PlayRandomClip(leverLatch);
    }
}
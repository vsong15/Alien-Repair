using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioClip footstep;
    public AudioClip hammer;

    public void CS_PlayFootstep()
    {
        GameSounds.instance.PlayClip(footstep);
    }

    public void CS_PlayHammer()
    {
        GameSounds.instance.PlayClip(hammer);
    }
}

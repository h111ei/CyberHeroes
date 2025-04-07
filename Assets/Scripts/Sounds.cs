using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip PressButton;

    public void ButtonSounds()
    {
        audioSource.PlayOneShot(PressButton);
    }
}

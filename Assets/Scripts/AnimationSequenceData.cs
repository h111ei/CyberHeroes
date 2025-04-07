using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationSequenceData", menuName = "Animation/Sequence Data")]
public class AnimationSequenceData : ScriptableObject
{
    public AudioClip[] audioClips;
    public float[] audioVolumes;
    public float[] delays;
}

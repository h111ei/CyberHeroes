// Core/MinigameConfig.cs
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameConfig : ScriptableObject
{
    public List<LevelData> levels;
    public AudioClip backgroundMusic;
    public float initialDelay = 1f;
}
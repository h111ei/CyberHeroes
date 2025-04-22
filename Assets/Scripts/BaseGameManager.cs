using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    public int currentLevelIndex { get; protected set; } = 0;
    public bool isGameOver { get; protected set; } = false;

    // Общие ссылки
    public Intro sequenceManager;
    public AudioSource backgroundMusic;

    // Общие методы
    public abstract void StartGame();
    public abstract void MoveToNextLevel();
    public abstract void RestartGame();
    public abstract void GameOver();

    // Общие вспомогательные методы
    protected virtual void PlaySequence(string sequenceName)
    {
        if (sequenceManager != null)
        {
            sequenceManager.PlaySequence(sequenceName);
        }
    }

    protected virtual void StopBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
    }
}

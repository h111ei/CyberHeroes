using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    // Общие поля для всех мини-игр
    public Intro AnimationManager;
    public AudioSource BackgroundMusic;

    protected int currentLevelIndex = 0;
    protected bool isGameOver = false;

    // Общие методы
    public abstract void StartGame();
    public abstract void MoveToNextLevel();
    public abstract void RestartGame();
    public abstract void GameOver();

    protected virtual void HandleLevelCompletion()
    {
        // Общая логика при завершении уровня
        if (BackgroundMusic != null)
        {
            BackgroundMusic.Stop();
        }
    }

    protected virtual void ResetGameState()
    {
        currentLevelIndex = 0;
        isGameOver = false;
    }
}

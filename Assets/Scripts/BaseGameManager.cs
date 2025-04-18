using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    // ����� ���� ��� ���� ����-���
    public Intro AnimationManager;
    public AudioSource BackgroundMusic;

    protected int currentLevelIndex = 0;
    protected bool isGameOver = false;

    // ����� ������
    public abstract void StartGame();
    public abstract void MoveToNextLevel();
    public abstract void RestartGame();
    public abstract void GameOver();

    protected virtual void HandleLevelCompletion()
    {
        // ����� ������ ��� ���������� ������
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

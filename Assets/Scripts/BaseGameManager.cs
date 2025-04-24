using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    [SerializeField] protected GameObject winPanel;
    [SerializeField] protected string sequenceAfterGame;
    [SerializeField] protected Intro AnimationManager;


    protected void HandleCompletion(int currentIndex, int totalCount, System.Action loadNextAction)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (currentIndex >= totalCount)
        {
            AnimationManager.PlaySequence(sequenceAfterGame);
        }
        else
        {
            loadNextAction?.Invoke();
        }
    }

    protected void MoveToNext(int currentLevel, int totalLevels, System.Action updateLevelAction)
    {
        if (currentLevel < totalLevels - 1)
        {
            updateLevelAction?.Invoke();
        }
        else
        {
            Debug.Log("The end");
            AnimationManager.PlaySequence(sequenceAfterGame);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    [SerializeField] protected GameObject winPanel;
    [SerializeField] protected string sequenceAfterGame;
    [SerializeField] protected Intro AnimationManager;


    protected void HandleCompletion(int currentIndex, int totalCount, System.Action loadNextAction, bool isLevelTransition = false)
    {
        

        if (currentIndex >= totalCount)
        {
            AnimationManager.PlaySequence(sequenceAfterGame);
        }
        else
        {
            if (isLevelTransition)
            {
                // Для перехода между уровнями используем оригинальную логику
                if (currentIndex < totalCount - 1)
                {
                    loadNextAction?.Invoke();
                }
                else
                {
                    Debug.Log("The end");
                    AnimationManager.PlaySequence(sequenceAfterGame);
                }
            }
            else
            {
                loadNextAction?.Invoke();
            }
        }
    }


}

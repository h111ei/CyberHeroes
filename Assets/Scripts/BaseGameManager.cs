using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class BaseGameManager : MonoBehaviour
{
    [SerializeField] protected string sequenceAfterGame;
    [SerializeField] protected Intro AnimationManager;


    protected void HandleCompletion(int currentIndex, int totalCount, System.Action loadNextAction, bool isLevelTransition = false, System.Action onBeforeCompletion = null)
    {
        onBeforeCompletion?.Invoke();

        if (currentIndex >= totalCount)
        {
            AnimationManager.PlaySequence(sequenceAfterGame);
        }
        else
        {
            if (isLevelTransition)
            {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public RectTransform BlackScreenAnim;
    public RectTransform canvas;

    public void OnClick() //Start transition
    {
        Vector2 targetPosition = CanvasAnimationUtils.GetOffscreenPosition(BlackScreenAnim, Vector2.left);
        BlackScreenAnim.gameObject.SetActive(true);
        CanvasAnimationUtils.AnimateCanvasBlackScreen(BlackScreenAnim, targetPosition, 2f, NewSceneLoad);

    }

    public void NewSceneLoad()
    {
        SceneManager.LoadScene(1);
    }
}

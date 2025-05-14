using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public RectTransform BlackScreenAnim;
    public RectTransform canvas;
    public GameObject CodePanel;
    public TMP_InputField inputField;
    public string correctCode = "1337";


    void Start()
    {
        inputField.onEndEdit.AddListener(CheckCode);
    }


    void CheckCode(string userInput)
    {
        if (userInput == correctCode)
        {
            OnClick();
            GameManager.isDevMode = true;
        }     
        inputField.text = "";
    }

    public void OnClick() //Start transition
    {
        Vector2 targetPosition = CanvasAnimationUtils.GetOffscreenPosition(BlackScreenAnim, Vector2.left);
        BlackScreenAnim.gameObject.SetActive(true);
        CanvasAnimationUtils.AnimateCanvasBlackScreen(BlackScreenAnim, targetPosition, 2f, NewSceneLoad);
        
    }
    public void CodePanelOn()
    {
        CodePanel.SetActive(true);
    }
    public void NewSceneLoad()
    {
        SceneManager.LoadScene(1);
    }


}

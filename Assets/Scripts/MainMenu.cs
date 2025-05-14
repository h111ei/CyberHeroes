
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public RectTransform BlackScreenAnim;
    public RectTransform canvas;
    public GameObject CodePanel;
    public TMP_InputField inputField;
    public string correctCode = "1337";
    public Button submitButton;
    public bool isMobilePlatform;

    void Start()
    {
        // Определяем платформу
        isMobilePlatform = Application.isMobilePlatform ||
                         Input.touchSupported &&
                         (Application.platform == RuntimePlatform.WebGLPlayer);

        inputField.onEndEdit.AddListener(CheckCode);

        // Настройка для мобильного ввода
        if (isMobilePlatform)
        {
            inputField.shouldHideMobileInput = false;
            submitButton.gameObject.SetActive(true);
            submitButton.onClick.AddListener(OnSubmitButtonClick);
        }
        else
        {
            submitButton.gameObject.SetActive(false);
        }
    }

    void OnSubmitButtonClick()
    {
        CheckCode(inputField.text);
    }

    void CheckCode(string userInput)
    {
        if (userInput == correctCode)
        {
            OnClick();
            GameManager.isDevMode = true;
        }
        inputField.text = "";

        if (isMobilePlatform)
        {
            inputField.DeactivateInputField();
        }
    }

    public void OnClick()
    {
        Vector2 targetPosition = CanvasAnimationUtils.GetOffscreenPosition(BlackScreenAnim, Vector2.left);
        BlackScreenAnim.gameObject.SetActive(true);
        CanvasAnimationUtils.AnimateCanvasBlackScreen(BlackScreenAnim, targetPosition, 2f, NewSceneLoad);
    }

    public void CodePanelOn()
    {
        CodePanel.SetActive(true);

        if (isMobilePlatform)
        {
            inputField.ActivateInputField();
            inputField.shouldHideMobileInput = false;
        }
        else
        {
            inputField.Select();
        }
    }

    public void CodePanelOff()
    {
        CodePanel.SetActive(false);

        if (isMobilePlatform)
        {
            inputField.DeactivateInputField();
        }
    }

    public void NewSceneLoad()
    {
        SceneManager.LoadScene(1);
    }
}
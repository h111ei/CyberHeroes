using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoSizeAndWrap : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
            enabled = false;
            return;
        }

        textMeshPro.enableWordWrapping = true;
        textMeshPro.alignment = TextAlignmentOptions.TopLeft;
    }


    // Example of dynamically changing text and ensuring wrapping:
    public void UpdateText(string newText)
    {
        textMeshPro.text = newText;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(Image))]
public class ProportionalImage : MonoBehaviour
{
    private void Start()
    {
        var image = GetComponent<Image>();
        var aspectFitter = GetComponent<AspectRatioFitter>();

        // �������� ����������� ������ ������������ ��������
        float aspectRatio = image.sprite.rect.width / image.sprite.rect.height;

        // ��������� ����������
        aspectFitter.aspectRatio = aspectRatio;
        aspectFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
    }
}

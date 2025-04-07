using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor;

public class WallGame : MonoBehaviour
{
    public RectTransform targetPanel; //  ���������� TargetPanel � Inspector
    public List<GameObject> buttonPrefabs; // ���������� ������� ������ � Inspector
    public Transform buttonSpawnPoint; // �����, ��� ����� ���������� ������
    public RectTransform buttonSpawnRect;
    public Canvas canvas; // ���������� Canvas � Inspector
    public GameObject EndPanel;
    public Vector3 offset = new Vector3(2, 0, 0);

    private DraggableCube correctButton;
    private List<GameObject> currentButtons = new List<GameObject>();





    void Start()
    {
        SpawnNewButtons();

    }

   
    
    void SpawnNewButtons()
    {
        Debug.Log("Q");
        foreach (GameObject prefab in currentButtons)
        {
            if(prefab.GetComponent<RectTransform>().anchoredPosition != targetPanel.anchoredPosition)
                Destroy(prefab);
        }
        currentButtons.Clear();


        Vector3 currentPosition = buttonSpawnPoint.position; // �������� � ��������� �������

        for (int i = 0; i < buttonPrefabs.Count; i++)
        {
           
                // ������� ��������� �������
                GameObject newObject = Instantiate(buttonPrefabs[i]);
                newObject.gameObject.SetActive(true);
                newObject.transform.SetParent(canvas.transform);
                newObject.GetComponent<RectTransform>().localScale = Vector3.one;
                 // ������������� �������
                  newObject.transform.position = currentPosition;

                // ������������� ������� (�� ��������� ��� ��������)
                newObject.transform.rotation = Quaternion.identity; // ��� ������� ������ �������
                
                // ��������� �������� ��� ���������� �������
                currentPosition += offset;

                // (�����������) ������ ������� GameObject ��������� ��� ������ �������:
                currentButtons.Add(newObject);

        }
    }




    public void CorrectButtonDragged(DraggableCube button)
    {
        Debug.Log("Correct!");
        //  ��������� ����������� ������ (��������, �������� ��������, ��������� ����)

        SpawnNewButtons();

        
    }

    public void IncorrectButtonDragged()
    {
        Debug.Log("Incorrect!");
        //  ��������� ������������� ������ (��������, �������� ���������, ������ ������)
        // ���������� ��� ������ �� �����
        EndPanel.gameObject.SetActive(true);
    }
}

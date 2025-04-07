using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor;

public class WallGame : MonoBehaviour
{
    public RectTransform targetPanel; //  Перетащите TargetPanel в Inspector
    public List<GameObject> buttonPrefabs; // Перетащите префабы кнопок в Inspector
    public Transform buttonSpawnPoint; // Точка, где будут появляться кнопки
    public RectTransform buttonSpawnRect;
    public Canvas canvas; // Перетащите Canvas в Inspector
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


        Vector3 currentPosition = buttonSpawnPoint.position; // Начинаем с начальной позиции

        for (int i = 0; i < buttonPrefabs.Count; i++)
        {
           
                // Создаем экземпляр префаба
                GameObject newObject = Instantiate(buttonPrefabs[i]);
                newObject.gameObject.SetActive(true);
                newObject.transform.SetParent(canvas.transform);
                newObject.GetComponent<RectTransform>().localScale = Vector3.one;
                 // Устанавливаем позицию
                  newObject.transform.position = currentPosition;

                // Устанавливаем поворот (по умолчанию без вращения)
                newObject.transform.rotation = Quaternion.identity; // Или укажите другой поворот
                
                // Добавляем смещение для следующего объекта
                currentPosition += offset;

                // (Опционально) Делаем текущий GameObject родителем для нового объекта:
                currentButtons.Add(newObject);

        }
    }




    public void CorrectButtonDragged(DraggableCube button)
    {
        Debug.Log("Correct!");
        //  Обработка правильного ответа (например, показать анимацию, начислить очки)

        SpawnNewButtons();

        
    }

    public void IncorrectButtonDragged()
    {
        Debug.Log("Incorrect!");
        //  Обработка неправильного ответа (например, показать сообщение, начать заново)
        // Возвращаем все кнопки на место
        EndPanel.gameObject.SetActive(true);
    }
}

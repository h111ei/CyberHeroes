using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class WallGameManager : MonoBehaviour
{
    public static WallGameManager Instance { get; private set; }
    public Intro Sequence;

    [System.Serializable]
    public class LevelSettings
    {
        public string[] panelTexts;
        public int[] correctPanelIndices;
        public int targetDropZoneIndex;
        public Sprite levelImage;
    }

    [Header("DropZone References")]
    [SerializeField] private DropZone[] _allDropZones;

    [Header("Level Settings")]
    [SerializeField] private LevelSettings[] _levels;
    [SerializeField] private Image _targetImage;

    private int _currentLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDropZones();
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
            // Не обновляем панели здесь, так как они могут еще не быть инициализированы
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDropZones()
    {

    }

    private void ActivateCurrentLevelDropZones()
    {
        if (_currentLevel < _levels.Length)
        {
            int zoneIndex = _levels[_currentLevel].targetDropZoneIndex;
            if (zoneIndex >= 0 && zoneIndex < _allDropZones.Length)
            {
                _allDropZones[zoneIndex].gameObject.SetActive(true);
            }
        }
    }

    public void MoveToNextLevel()
    {
        if (_currentLevel < _levels.Length - 1)
        {
            _currentLevel++;
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
            UpdateAllPanelsText();
        }
        else
        {
            Debug.Log("The end");
            Sequence.PlaySequence("TosecondRobot");
        }
    }

    private void UpdateLevelImage()
    {
        if (_targetImage != null && _currentLevel < _levels.Length)
        {
            _targetImage.sprite = _levels[_currentLevel].levelImage;
            _targetImage.preserveAspect = true;
        }
    }

    public string GetTextForPanel(int panelIndex)
    {
        if (_levels == null ||
            _currentLevel >= _levels.Length ||
            panelIndex >= _levels[_currentLevel].panelTexts.Length)
            return "";

        return _levels[_currentLevel].panelTexts[panelIndex];
    }

    private void UpdateAllPanelsText()
    {
        // Находим все панели на сцене
        DragAndDropController[] allPanels = FindObjectsOfType<DragAndDropController>();
        foreach (var panel in allPanels)
        {
            panel.UpdateTextFromManager();
        }
    }

    public int GetCurrentLevel() => _currentLevel;
}

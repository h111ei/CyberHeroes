using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class WallGameManager : BaseGameManager
{
    public static WallGameManager Instance { get; private set; }

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
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
        }
        else
        {
            Destroy(gameObject);
        }
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
        HandleCompletion(
            _currentLevel,
            _levels.Length,
            () =>
            {
                _currentLevel++;
                UpdateLevelImage();
                ActivateCurrentLevelDropZones();
                UpdateAllPanelsText();
            },
            false
        );
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
    public bool CanAttachToDropZone(DropZone dropZone, int panelIndex)
    {
        if (_currentLevel >= _levels.Length) return false;

        // Проверяем что:
        // 1. Это правильная панель для уровня
        // 2. Это правильный DropZone для уровня
        int correctZoneIndex = _levels[_currentLevel].targetDropZoneIndex;
        bool isCorrectPanel = System.Array.IndexOf(_levels[_currentLevel].correctPanelIndices, panelIndex) >= 0;
        bool isCorrectZone = _allDropZones[correctZoneIndex] == dropZone;

        return isCorrectPanel && isCorrectZone;
    }



    private void UpdateAllPanelsText()
    {
        DragAndDropController[] allPanels = FindObjectsOfType<DragAndDropController>();
        foreach (var panel in allPanels)
        {
            panel.UpdateTextFromManager();
        }
    }

    public int GetCurrentLevel() => _currentLevel;
}
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDropZones();
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void StartGame()
    {
        ResetGameState();
        UpdateLevelImage();
        ActivateCurrentLevelDropZones();
    }

    private void InitializeDropZones() { }

    private void ActivateCurrentLevelDropZones()
    {
        if (currentLevelIndex < _levels.Length)
        {
            int zoneIndex = _levels[currentLevelIndex].targetDropZoneIndex;
            if (zoneIndex >= 0 && zoneIndex < _allDropZones.Length)
            {
                _allDropZones[zoneIndex].gameObject.SetActive(true);
            }
        }
    }

    public override void MoveToNextLevel()
    {
        if (currentLevelIndex < _levels.Length - 1)
        {
            currentLevelIndex++;
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
            UpdateAllPanelsText();
        }
        else
        {
            Debug.Log("The end");
            AnimationManager.PlaySequence("TosecondRobot");
            HandleLevelCompletion();
        }
    }

    private void UpdateLevelImage()
    {
        if (_targetImage != null && currentLevelIndex < _levels.Length)
        {
            _targetImage.sprite = _levels[currentLevelIndex].levelImage;
            _targetImage.preserveAspect = true;
        }
    }

    public string GetTextForPanel(int panelIndex)
    {
        if (_levels == null || currentLevelIndex >= _levels.Length ||
            panelIndex >= _levels[currentLevelIndex].panelTexts.Length)
            return "";
        return _levels[currentLevelIndex].panelTexts[panelIndex];
    }

    public bool CanAttachToDropZone(DropZone dropZone, int panelIndex)
    {
        if (currentLevelIndex >= _levels.Length) return false;

        int correctZoneIndex = _levels[currentLevelIndex].targetDropZoneIndex;
        bool isCorrectPanel = System.Array.IndexOf(_levels[currentLevelIndex].correctPanelIndices, panelIndex) >= 0;
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

    public override void GameOver()
    {
        isGameOver = true;
        // Логика завершения игры для WallGame
    }

    public override void RestartGame()
    {
        ResetGameState();
        // Дополнительная логика рестарта для WallGame
        UpdateLevelImage();
        ActivateCurrentLevelDropZones();
        UpdateAllPanelsText();
    }
}

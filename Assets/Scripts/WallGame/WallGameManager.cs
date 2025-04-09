using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallGameManager : MonoBehaviour
{
    public static WallGameManager Instance { get; private set; }
    public Intro Sequence;

    [System.Serializable]
    public class LevelSettings
    {
        public string[] panelTexts;
        public int[] correctPanelIndices; // ������� �������, ������� ����� ����������
        public int targetDropZoneIndex; // ������ DropZone �� ������ �������
        public Sprite levelImage;
    }

    [Header("DropZone References")]
    [SerializeField] private DropZone[] _allDropZones; // ��� DropZone �� �����

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
       
        // ���������� ������ ������ ��� �������� ������
        if (_currentLevel < _levels.Length)
        {
            int zoneIndex = _levels[_currentLevel].targetDropZoneIndex;
            if (zoneIndex >= 0 && zoneIndex < _allDropZones.Length)
            {
                _allDropZones[zoneIndex].gameObject.SetActive(true);
            }
        }
    }

    public bool CanAttachToDropZone(DropZone dropZone, int panelIndex)
    {
        if (_currentLevel >= _levels.Length) return false;

        // ��������� ���:
        // 1. ��� ���������� ������ ��� ������
        // 2. ��� ���������� DropZone ��� ������
        int correctZoneIndex = _levels[_currentLevel].targetDropZoneIndex;
        bool isCorrectPanel = System.Array.IndexOf(_levels[_currentLevel].correctPanelIndices, panelIndex) >= 0;
        bool isCorrectZone = _allDropZones[correctZoneIndex] == dropZone;

        return isCorrectPanel && isCorrectZone;
    }

    public void MoveToNextLevel()
    {
        if (_currentLevel < _levels.Length - 1)
        {
            _currentLevel++;
            UpdateLevelImage();
            ActivateCurrentLevelDropZones();
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

    public int GetCurrentLevel() => _currentLevel;
}

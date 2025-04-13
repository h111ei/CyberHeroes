using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{

    [System.Serializable]
    public class AnimationSequence
    {
        public string name;
        public List<AnimationStep> steps;
        public bool loop;
        public LoopType loopType;
    }

    [System.Serializable]
    public class AnimationStep
    {
        public enum StepType
        {
            ActivateObject,
            DeactivateObject,
            Move,
            Scale,
            Fade,
            PlaySound,
            Delay,
            Callback,
            LoadSequence
        }

        public StepType type;
        public GameObject targetObject;
        public Vector3 targetValue;
        public float duration;
        public Ease easeType = Ease.Linear;
        public AudioClip soundClip;
        public float soundVolume = 1f;
        public System.Action customCallback;
        public string targetSequenceName;
    }

    [Header("References")]
    public Canvas mainCanvas;
    public AudioSource mainAudioSource;
    public AudioSource backgroundAudioSource;

    [Header("Animation Sequences")]
    public List<AnimationSequence> sequences;

    [Header("UI Elements")]
    public RectTransform blackScreen;
    public RectTransform introLogo;
    public GameObject[] virusObjects;
    public Image alertImage;
    // другие UI элементы

    private Dictionary<string, Sequence> activeSequences = new Dictionary<string, Sequence>();

    private void Start()
    {
        InitializeIntroAnimation();
    }

    private void InitializeIntroAnimation()
    {
        float maxScale = CalculateMaxScale(introLogo);
        Vector2 targetPosition = CanvasAnimationUtils.GetOffscreenPosition(
        blackScreen,
        Vector2.right
        );

        CanvasAnimationUtils.AnimateCanvasBlackScreen(blackScreen, targetPosition, 2f, () =>
        {
            PlaySequence("IntroSequence");
        });
    }

    private float CalculateMaxScale(RectTransform target)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector2 objectSize = target.sizeDelta;

        float maxScaleX = screenWidth / objectSize.x;
        float maxScaleY = screenHeight / objectSize.y;
        return Mathf.Min(maxScaleX, maxScaleY);
    }

    public void PlaySequence(string sequenceName)
    {
        if (activeSequences.ContainsKey(sequenceName))
        {
            activeSequences[sequenceName].Kill();
            activeSequences.Remove(sequenceName);
        }

        AnimationSequence sequence = sequences.Find(s => s.name == sequenceName);
        if (sequence == null) return;

        Sequence newSequence = DOTween.Sequence();

        foreach (var step in sequence.steps)
        {
            switch (step.type)
            {
                case AnimationStep.StepType.ActivateObject:
                    newSequence.AppendCallback(() => step.targetObject.SetActive(true));
                    break;

                case AnimationStep.StepType.DeactivateObject:
                    newSequence.AppendCallback(() => step.targetObject.SetActive(false));
                    break;

                case AnimationStep.StepType.Move:
                    if (step.targetObject.TryGetComponent<RectTransform>(out var rectTransform))
                    {
                        newSequence.Append(rectTransform.DOMove(step.targetValue, step.duration).SetEase(step.easeType));
                    }
                    break;

                case AnimationStep.StepType.Scale:
                    newSequence.Append(step.targetObject.transform.DOScale(step.targetValue, step.duration).SetEase(step.easeType));
                    break;

                case AnimationStep.StepType.Fade:
                    if (step.targetObject.TryGetComponent<Image>(out var image))
                    {
                        newSequence.Append(image.DOFade(step.targetValue.x, step.duration).SetEase(step.easeType));
                    }
                    break;

                case AnimationStep.StepType.PlaySound:
                    newSequence.AppendCallback(() => mainAudioSource.PlayOneShot(step.soundClip, step.soundVolume));
                    break;

                case AnimationStep.StepType.Delay:
                    newSequence.AppendInterval(step.duration);
                    break;

                case AnimationStep.StepType.Callback:
                    newSequence.AppendCallback(() => step.customCallback?.Invoke());
                    break;
                case AnimationStep.StepType.LoadSequence:
                    // Запоминаем текущую последовательность
                    string currentSequence = sequenceName;

                    // Добавляем callback для загрузки новой последовательности
                    newSequence.AppendCallback(() => {
                        // Проверяем, не пытаемся ли загрузить ту же последовательность
                        if (step.targetSequenceName != currentSequence)
                        {
                            this.PlaySequence(step.targetSequenceName);
                        }
                    });
                    break;
            }
        }

        if (sequence.loop)
        {
            newSequence.SetLoops(-1, sequence.loopType);
        }

        activeSequences.Add(sequenceName, newSequence);
    }

    public void StopSequence(string sequenceName)
    {
        if (activeSequences.TryGetValue(sequenceName, out var sequence))
        {
            sequence.Kill();
            activeSequences.Remove(sequenceName);
        }
    }

    // Примеры упрощенных методов
    public void StartDownloadingAntiVirus()
    {
        StopSequence("RedAlert");
        StopSequence("Virus");
        PlaySequence("DownloadAntiVirus");
    }

    public void StartTutorial()
    {
        PlaySequence("RobotTutorial");
    }

    public void StartGameSequence()
    {
        PlaySequence("GameStart");
    }

}

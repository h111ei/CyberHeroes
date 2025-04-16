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
    public class AnimationTemplate
    {
        public string templateName;
        public List<AnimationStep> templateSteps;
    }

    [System.Serializable]
    public class AnimationSequence
    {
        public string name;
        public string basedOnTemplate; // »м€ шаблона, на котором основана эта последовательность
        public List<AnimationStepOverride> stepOverrides; // ѕереопределени€ дл€ шаблона
        public bool loop;
        public LoopType loopType;
    }

    [System.Serializable]
    public class AnimationStepOverride
    {
        public int stepIndex; // »ндекс шага в шаблоне, который нужно переопределить
        public AnimationStep overriddenStep; // Ќовые параметры дл€ шага
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

    [Header("Templates")]
    public List<AnimationTemplate> templates;

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

        // ѕолучаем список шагов - либо из шаблона, либо напр€мую из последовательности
        List<AnimationStep> stepsToPlay = GetStepsForSequence(sequence);

        Sequence newSequence = DOTween.Sequence();

        foreach (var step in stepsToPlay)
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
                    // «апоминаем текущую последовательность
                    string currentSequence = sequenceName;

                    // ƒобавл€ем callback дл€ загрузки новой последовательности
                    newSequence.AppendCallback(() => {
                        // ѕровер€ем, не пытаемс€ ли загрузить ту же последовательность
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

    private List<AnimationStep> GetStepsForSequence(AnimationSequence sequence)
    {
        // ≈сли последовательность не основана на шаблоне, возвращаем еЄ собственные шаги
        if (string.IsNullOrEmpty(sequence.basedOnTemplate))
        {
            return sequence.steps;
        }

        // Ќаходим шаблон
        var template = templates.Find(t => t.templateName == sequence.basedOnTemplate);
        if (template == null)
        {
            Debug.LogWarning($"Template {sequence.basedOnTemplate} not found for sequence {sequence.name}");
            return sequence.steps;
        }

        // —оздаем копию шагов из шаблона
        List<AnimationStep> resultSteps = new List<AnimationStep>();
        foreach (var step in template.templateSteps)
        {
            resultSteps.Add(new AnimationStep()
            {
                type = step.type,
                targetObject = step.targetObject,
                targetValue = step.targetValue,
                duration = step.duration,
                easeType = step.easeType,
                soundClip = step.soundClip,
                soundVolume = step.soundVolume,
                customCallback = step.customCallback,
                targetSequenceName = step.targetSequenceName
            });
        }

        // ѕримен€ем переопределени€
        if (sequence.stepOverrides != null)
        {
            foreach (var overrideStep in sequence.stepOverrides)
            {
                if (overrideStep.stepIndex >= 0 && overrideStep.stepIndex < resultSteps.Count)
                {
                    var originalStep = resultSteps[overrideStep.stepIndex];
                    var newStep = overrideStep.overriddenStep;

                    // ѕримен€ем только измененные пол€
                    if (newStep.targetObject != null) originalStep.targetObject = newStep.targetObject;
                    if (newStep.targetValue != Vector3.zero) originalStep.targetValue = newStep.targetValue;
                    if (newStep.duration > 0) originalStep.duration = newStep.duration;
                    if (newStep.easeType != Ease.Linear) originalStep.easeType = newStep.easeType;
                    if (newStep.soundClip != null) originalStep.soundClip = newStep.soundClip;
                    if (newStep.soundVolume != 1f) originalStep.soundVolume = newStep.soundVolume;
                    if (newStep.customCallback != null) originalStep.customCallback = newStep.customCallback;
                    if (!string.IsNullOrEmpty(newStep.targetSequenceName)) originalStep.targetSequenceName = newStep.targetSequenceName;
                }
            }
        }

        return resultSteps;
    }

    public void StopSequence(string sequenceName)
    {
        if (activeSequences.TryGetValue(sequenceName, out var sequence))
        {
            sequence.Kill();
            activeSequences.Remove(sequenceName);
        }
    }

    // ѕримеры упрощенных методов
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
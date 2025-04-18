using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public List<AnimationStep> steps;
    }

    [System.Serializable]
    public class AnimationSequence
    {
        public string name;
        public string basedOnTemplate; // Template name this sequence is based on
        public List<AnimationStep> steps; // Steps if sequence isn't based on template
        public List<AnimationStepOverride> stepOverrides; // Overrides for template steps
        public bool loop;
        public LoopType loopType;
    }

    [System.Serializable]
    public class AnimationStepOverride
    {
        public enum OverrideType
        {
            ModifyExisting, 
            InsertAfter,    
            InsertBefore,   
            Append          
        }

        public OverrideType overrideType = OverrideType.ModifyExisting;
        public int stepIndex; 
        public AnimationStep overriddenStep; 
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
        public UnityEvent customCallback;
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

    // Calculate maximum scale to fit object within screen
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
                    else if (step.targetObject.TryGetComponent<TextMeshProUGUI>(out var text))
                    {
                        newSequence.Append(text.DOFade(step.targetValue.x, step.duration).SetEase(step.easeType));
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
                    string currentSequence = sequenceName;
                    newSequence.AppendCallback(() => {
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
        // If sequence isn't based on template, use its own steps
        if (string.IsNullOrEmpty(sequence.basedOnTemplate))
        {
            return new List<AnimationStep>(sequence.steps);
        }

        // Find template
        var template = templates.Find(t => t.templateName == sequence.basedOnTemplate);
        if (template == null)
        {
            Debug.LogWarning($"Template {sequence.basedOnTemplate} not found for sequence {sequence.name}");
            return new List<AnimationStep>(sequence.steps);
        }


        List<AnimationStep> resultSteps = new List<AnimationStep>();
        foreach (var step in template.steps)
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

        // Apply overrides
        if (sequence.stepOverrides != null && sequence.stepOverrides.Count > 0)
        {
            // Sort overrides by index and type
            var sortedOverrides = sequence.stepOverrides.OrderBy(o => o.stepIndex).ThenBy(o => o.overrideType).ToList();

            // Apply overrides in reverse order to maintain correct indices
            for (int i = sortedOverrides.Count - 1; i >= 0; i--)
            {
                var overrideStep = sortedOverrides[i];

                switch (overrideStep.overrideType)
                {
                    case AnimationStepOverride.OverrideType.ModifyExisting:
                        if (overrideStep.stepIndex >= 0 && overrideStep.stepIndex < resultSteps.Count)
                        {
                            var originalStep = resultSteps[overrideStep.stepIndex];
                            ApplyStepOverride(originalStep, overrideStep.overriddenStep);
                        }
                        break;

                    case AnimationStepOverride.OverrideType.InsertAfter:
                        if (overrideStep.stepIndex >= -1 && overrideStep.stepIndex < resultSteps.Count)
                        {
                            int insertIndex = overrideStep.stepIndex + 1;
                            if (insertIndex > resultSteps.Count) insertIndex = resultSteps.Count;
                            resultSteps.Insert(insertIndex, overrideStep.overriddenStep);
                        }
                        break;

                    case AnimationStepOverride.OverrideType.InsertBefore:
                        if (overrideStep.stepIndex >= 0 && overrideStep.stepIndex <= resultSteps.Count)
                        {
                            resultSteps.Insert(overrideStep.stepIndex, overrideStep.overriddenStep);
                        }
                        break;

                    case AnimationStepOverride.OverrideType.Append:
                        resultSteps.Add(overrideStep.overriddenStep);
                        break;
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
    private void ApplyStepOverride(AnimationStep original, AnimationStep overrides)
    {
        if (overrides.targetObject != null) original.targetObject = overrides.targetObject;
        if (overrides.targetValue != Vector3.zero) original.targetValue = overrides.targetValue;
        if (overrides.duration > 0) original.duration = overrides.duration;
        if (overrides.easeType != Ease.Linear) original.easeType = overrides.easeType;
        if (overrides.soundClip != null) original.soundClip = overrides.soundClip;
        if (overrides.soundVolume != 1f) original.soundVolume = overrides.soundVolume;
        if (overrides.customCallback != null) original.customCallback = overrides.customCallback;
        if (!string.IsNullOrEmpty(overrides.targetSequenceName)) original.targetSequenceName = overrides.targetSequenceName;
    }
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
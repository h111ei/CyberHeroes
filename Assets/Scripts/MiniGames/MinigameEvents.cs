using System;

public static class MinigameEvents
{
    public static event Action<IMinigame> OnMinigameStart;
    public static event Action<bool> OnMinigameEnd;
    public static event Action OnMinigameRestart;

    public static void TriggerMinigameStart(IMinigame minigame) => OnMinigameStart?.Invoke(minigame);
    public static void TriggerMinigameEnd(bool isWin) => OnMinigameEnd?.Invoke(isWin);
    public static void TriggerMinigameRestart() => OnMinigameRestart?.Invoke();
}
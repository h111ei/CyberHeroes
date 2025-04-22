
using UnityEngine;

public enum MinigameType { Quiz, DragAndDrop }

public interface IMinigame
{
    MinigameType Type { get; }
    void Initialize();
    void StartGame();
    void EndGame(bool isWin);
    void Restart();
    void Cleanup();
}

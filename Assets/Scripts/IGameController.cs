using System;
using UnityEngine;

public interface IGameController
{
    event Func<Vector3, int> OnTriggerTap;
    event Func<Vector3, int> OnTriggerHold;

    event Func<Vector3, bool> OnInitialTrigger;
    event Action OnStartGame;
    event Action OnPauseGame;
    event Action OnResumeGame;
    event Action<LevelData> OnRestartGame;
    event Action<LevelData> OnLevelChanged;
    event Action OnWinGame;
    event Action OnStopGame;
}
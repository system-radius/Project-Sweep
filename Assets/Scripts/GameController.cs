using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    public event Func<Vector3, bool> OnInitialTrigger;
    public event Action OnStartGame;
    public event Action OnPauseGame;
    public event Action OnResumeGame;
    public event Action<LevelData> OnRestartGame;
    public event Action<LevelData> OnLevelChanged;
    public event Action OnWinGame;
    public event Action OnStopGame;

    public event Func<Vector3, int> OnTriggerTap;
    public event Func<Vector3, int> OnTriggerHold;

    [SerializeField]
    private TouchManager touchManager;

    [SerializeField]
    private GameState state;
    private GameState prePauseState;

    [SerializeField]
    private LevelSelection levelSelection;

    private LevelData level;

    private bool fromPause = false;

    private void Awake()
    {
        level = levelSelection.GetDefaultLevelData();
    }

    private void OnEnable()
    {
        touchManager.OnTriggerTap += TriggerTap;
        touchManager.OnTriggerHold += TriggerHold;
        touchManager.OnTriggerPause += PauseGame;
        touchManager.OnTriggerResume += ResumeGame;
        touchManager.OnTriggerRestart += RestartGame;
        OnStartGame += StartGame;
        OnStopGame += StopGame;

        levelSelection.LevelChange += LevelChanged;
    }

    private void OnDisable()
    {
        touchManager.OnTriggerTap -= TriggerTap;
        touchManager.OnTriggerHold -= TriggerHold;
        touchManager.OnTriggerPause -= PauseGame;
        touchManager.OnTriggerResume -= ResumeGame;
        touchManager.OnTriggerRestart -= RestartGame;
        OnStartGame -= StartGame;
        OnStopGame -= StopGame;

        levelSelection.LevelChange -= LevelChanged;
    }

    private void LevelChanged(LevelData newLevel)
    {
        level = newLevel;
        //Debug.Log("Level changed: " + newLevel.displayName);
        OnLevelChanged?.Invoke(newLevel);
    }

    private void Start()
    {
        RestartGame();
    }

    private void RestartGame()
    {
        //Debug.Log("Restarting game!");
        OnRestartGame?.Invoke(level);
        OnResumeGame?.Invoke();
        state = GameState.START;
    }

    private void ResumeGame()
    {
        if (GameState.PAUSED != state) return;
        state = prePauseState;
        OnResumeGame?.Invoke();
        if (GameState.PLAY == state) OnStartGame?.Invoke();
    }

    private void PauseGame()
    {
        if (GameState.PAUSED == state) return;

        prePauseState = state;
        state = GameState.PAUSED;
        OnPauseGame?.Invoke();
    }

    private void StartGame()
    {
        state = GameState.PLAY;
    }

    private void StopGame()
    {
        state = GameState.GAME_OVER;
    }

    private bool VerifyState(Vector3 position)
    {
        if (GameState.GAME_OVER == state || GameState.PAUSED == state) return false;

        if ((GameState.START == state && (OnInitialTrigger?.Invoke(position) ?? false)))
        {
            OnStartGame?.Invoke();
        }

        return true;
    }

    public void TriggerTap(Vector3 position)
    {
        if (GameState.START == state)
        {
            // On start, trigger hold instead of tap regardless of the action.
            TriggerHold(position);
            return;
        }

        TriggerTouchEvent(position, OnTriggerTap);
    }

    public void TriggerHold(Vector3 position)
    {
        TriggerTouchEvent(position, OnTriggerHold);
    }

    private void TriggerTouchEvent(Vector3 position, Func<Vector3, int> touchEvent)
    {
        if (!VerifyState(position)) return;

        int touchState = (touchEvent?.Invoke(position) ?? -1);
        // This call will only acknowledge the last invoked function for the return value.
        if (touchState != 0)
        {
            OnStopGame?.Invoke();
            if (touchState > 0)
            {
                OnWinGame?.Invoke();
            }
        }

        // To allow processing of the individual subscribers to the event, this pattern can be used:
        /*
        foreach (Func<Vector3, bool> f in OnTriggerTap.GetInvocationList()) {
            // Calling f() by itself will invoke the subscribed function, and its return value will be available for checking.
            bool continuePlay = f(position);
        }
        */
    }
}

public enum GameState
{
    START,
    PLAY,
    PAUSED,
    GAME_OVER
}

public enum GameLevel
{
    EASY,
    MEDIUM,
    EXPERT
}
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    [SerializeField]
    private TextMeshProUGUI minutesTens;
    [SerializeField]
    private TextMeshProUGUI minutesOnes;
    [SerializeField]
    private TextMeshProUGUI secondsTens;
    [SerializeField]
    private TextMeshProUGUI secondsOnes;

    private float totalTime = 0;
    private bool started = false;
    private bool paused = false;

    private void Awake()
    {
        gameController = gameControllerObject as IGameController;
        if (gameController == null)
        {
            throw new System.Exception("GameController not found!");
        }
    }

    private void OnEnable()
    {
        gameController.OnStartGame += StartTimer;
        gameController.OnPauseGame += PauseTimer;
        gameController.OnStopGame += StopTimer;
        gameController.OnRestartGame += RestartTimer;
    }

    private void OnDisable()
    {
        gameController.OnStartGame -= StartTimer;
        gameController.OnPauseGame -= PauseTimer;
        gameController.OnStopGame -= StopTimer;
        gameController.OnRestartGame -= RestartTimer;
    }

    private void DeriveTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        int minuteTensValue = minutes / 10;
        int minuteOnesValue = minutes % 10;

        minutesTens.text = minuteTensValue.ToString();
        minutesOnes.text = minuteOnesValue.ToString();

        int secondsTensValue = seconds / 10;
        int secondsOnesValue = seconds % 10;

        secondsTens.text = secondsTensValue.ToString();
        secondsOnes.text = secondsOnesValue.ToString();
    }

    private void RestartTimer(LevelData data = null)
    {
        Debug.Log("Timer restarted");
        totalTime = 0;
        DeriveTime(totalTime);
    }

    private void StartTimer()
    {
        if (paused)
        {
            paused = false;
            Debug.Log("Timer unpaused!");
            return;
        }
        if (started) return;
        Debug.Log("Timer started!");
        started = true;
        totalTime = 0;
    }

    private void PauseTimer()
    {
        if (!started) return;
        Debug.Log("Timer paused!");
        paused = true;
    }

    private void StopTimer()
    {
        Debug.Log("Timer Stopped!");
        started = false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (started && !paused)
        {
            totalTime += Time.fixedDeltaTime;
            DeriveTime(totalTime);
        }
    }

    public float GetCurrentTime()
    {
        return totalTime;
    }

    public string GetCurrentTimeString()
    {
        return minutesTens.text + minutesOnes.text + ":" + secondsTens.text + secondsOnes.text;
    }
}

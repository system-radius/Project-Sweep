using System;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreController : MonoBehaviour
{
    [SerializeField]
    private List<HighScoreList> highScorePanel = new();

    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    [SerializeField]
    private Timer timer;

    private int activeHighScorePanelIndex = -1;

    private void Awake()
    {
        gameController = gameControllerObject as IGameController;
        if (gameController == null)
        {
            throw new System.Exception("GameController not found!");
        }

        LoadHighScores();
    }

    private void OnEnable()
    {
        gameController.OnRestartGame += OnLevelChanged;
        gameController.OnLevelChanged += OnLevelChanged;
        gameController.OnWinGame += OnWin;
    }

    public void OnDisable()
    {
        gameController.OnRestartGame -= OnLevelChanged;
        gameController.OnLevelChanged -= OnLevelChanged;
        gameController.OnWinGame -= OnWin;
    }

    private void OnLevelChanged(LevelData level)
    {
        if (activeHighScorePanelIndex >= 0)
        {
            highScorePanel[activeHighScorePanelIndex].gameObject.SetActive(false);
        }

        activeHighScorePanelIndex = level.id;
        highScorePanel[activeHighScorePanelIndex].gameObject.SetActive(true);
    }

    private void OnWin()
    {
        if (activeHighScorePanelIndex < 0) return;

        HighScoreList list = highScorePanel[activeHighScorePanelIndex];
        float time = timer.GetCurrentTime();
        int index = 0;
        for (; index < list.HighScores.Count; index++)
        {
            HighScoreData data = list.HighScores[index];
            if (time < data.GetParsedTime())
            {
                // Ask for the player's name.
                list.InsertHighScore("AAA", timer.GetCurrentTimeString(), index);
                break;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveHighScores();
        } else
        {

        }
    }

    private void LoadHighScores()
    {
        Debug.LogError("Loading data!");
        for (int i = 0; i < highScorePanel.Count; i++)
        {
            if (!PlayerPrefs.HasKey(i.ToString())) continue;
            string json = PlayerPrefs.GetString(i.ToString());
            HighScoreSaveList saveList = JsonUtility.FromJson<HighScoreSaveList>(json);
            highScorePanel[i].LoadHighScores(saveList);
        }
    }

    private void SaveHighScores()
    {
        // Save the data
        Debug.LogError("Saving data!");
        for (int i = 0; i < highScorePanel.Count; i++)
        {
            HighScoreList list = highScorePanel[i];
            HighScoreSaveList saveList = CreateSaveList(list);
            string jsonString = JsonUtility.ToJson(saveList);
            PlayerPrefs.SetString(i.ToString(), jsonString);
            Debug.Log(jsonString);
        }
    }

    private HighScoreSaveList CreateSaveList(HighScoreList list)
    {
        HighScoreSaveList saveList = new HighScoreSaveList();
        foreach (HighScoreData data in list.HighScores)
        {
            saveList.HighScores.Add(new HighScoreSaveData(data.Name, data.Time));
        }

        return saveList;
    }
}

[Serializable]
public class HighScoreSaveList
{
    public List<HighScoreSaveData> HighScores = new();
}

[Serializable]
public class HighScoreSaveData
{
    public string name;
    public string time;

    public HighScoreSaveData(string name, string time)
    {
        this.name = name;
        this.time = time;
    }
}
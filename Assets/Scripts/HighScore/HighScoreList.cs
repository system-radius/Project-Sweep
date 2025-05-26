using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class HighScoreList : MonoBehaviour
{
    [SerializeField]
    private List<HighScoreData> highScores = new();

    public ReadOnlyCollection<HighScoreData> HighScores {  get { return highScores.AsReadOnly(); } }

    public void InsertHighScore(string name, string time, int index)
    {
        // Starting at the provided index, all scores will be moved down.
        for (int i = highScores.Count - 1; i > index; i--) {
            if (i - 1 < 0) break;
            HighScoreData data = highScores[i];
            HighScoreData highData = highScores[i - 1];
            string subName = highData.Name;
            string subTime = highData.Time;
            data.SetData(subName, subTime);
        }

        highScores[index].SetData(name, time);
    }

    public void LoadHighScores(HighScoreSaveList saveList)
    {
        for (int i = 0; i < saveList.HighScores.Count; i++)
        {
            highScores[i].SetData(saveList.HighScores[i].name, saveList.HighScores[i].time);
        }
    }
}
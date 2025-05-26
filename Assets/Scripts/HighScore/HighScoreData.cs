using TMPro;
using UnityEngine;

public class HighScoreData : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameEntry;
    public string Name
    {
        get { return nameEntry.text; }
    }

    [SerializeField]
    private TextMeshProUGUI timeEntry;
    public string Time
    {
        get { return timeEntry.text; }
    }

    private float parsedTime;

    private void Awake()
    {
        parsedTime = ParseTime(Time);
    }

    public void SetData(string name, string time)
    {
        nameEntry.text = name;
        timeEntry.text = time;
        parsedTime = ParseTime(time);
    }

    public float ParseTime(string time)
    {
        string[] parts = time.Split(':');
        float seconds = -1;

        if (parts.Length == 1)
        {
            seconds = float.Parse(parts[0]);
        } else if (parts.Length == 2)
        {
            seconds = (float.Parse(parts[0]) * 60f) + float.Parse(parts[1]);
        }

        return seconds < 0 ? 5999f : seconds;
    }

    public float GetParsedTime()
    {
        return parsedTime;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Game Data/Level Data")]
public class LevelData : ScriptableObject
{
    public int id;
    public string displayName;
    public int sizeX;
    public int sizeY;
    public int mines;

    public float orthographicSize;
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LevelSelection : MonoBehaviour
{
    public event Action<LevelData> LevelChange;

    private TMP_Dropdown dropdown;

    [SerializeField]
    private List<LevelData> levels = new();

    public int selectedIndex = 0;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options = CreateOptions();

        dropdown.onValueChanged.AddListener(OnDropdownChange);
    }

    private List<TMP_Dropdown.OptionData> CreateOptions()
    {
        List<TMP_Dropdown.OptionData> options = new();

        foreach (LevelData level in levels)
        {
            options.Add(new TMP_Dropdown.OptionData(level.displayName));
        }

        return options;
    }

    private void OnDropdownChange(int index)
    {
        selectedIndex = index;
        LevelChange?.Invoke(levels[index]);
        SaveData();
    }

    public LevelData GetDefaultLevelData()
    {
        selectedIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        dropdown.onValueChanged.RemoveListener(OnDropdownChange);
        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(OnDropdownChange);
        return levels[selectedIndex];
    }

    private void SaveData()
    {
        Debug.LogError("Saving level data!");
        PlayerPrefs.SetInt("SavedLevel", selectedIndex);
        PlayerPrefs.Save();
    }
}
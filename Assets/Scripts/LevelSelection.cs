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
        LevelChange?.Invoke(levels[index]);
    }

    public LevelData GetDefaultLevelData()
    {
        return levels[0];
    }
}
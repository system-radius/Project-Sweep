using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameEntry : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Button submitButton;

    private bool submitted = false;
    private string inputText = "";

    private void Awake()
    {
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnDisable()
    {
        inputField.DeactivateInputField();
    }

    private void OnSubmit()
    {
        inputText = inputField.text;
        submitted = true;
    }

    public void AskPlayerName(HighScoreList list, string time, int index)
    {
        gameObject.SetActive(true);
        StartCoroutine(EnterPlayerName(list, time, index));
    }

    IEnumerator EnterPlayerName(HighScoreList list, string time, int index)
    {
        string input = PlayerPrefs.GetString("DefaultName");
        if (String.Empty.Equals(input))
        {
            input = "AAA";
        }

        inputField.text = input;

        submitted = false;
        yield return new WaitUntil(() => submitted);

        if (String.Empty.Equals(inputText)) {
            inputText = input;
        }

        list.InsertHighScore(inputText, time, index);
        PlayerPrefs.SetString("DefaultName", inputText);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}
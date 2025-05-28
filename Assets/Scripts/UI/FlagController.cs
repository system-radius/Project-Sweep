using TMPro;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI dashText;

    [SerializeField]
    private TextMeshProUGUI hundredsText;

    [SerializeField]
    private TextMeshProUGUI tensText;

    [SerializeField]
    private TextMeshProUGUI onesText;

    [SerializeField]
    private MineField mineField;

    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    private int flagCount = 0;

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
        gameController.OnRestartGame += SetFlagCount;
        mineField.OnUpdateFlagCount += UpdateFlagCount;
    }

    private void OnDisable()
    {
        gameController.OnRestartGame -= SetFlagCount;
        mineField.OnUpdateFlagCount -= UpdateFlagCount;
    }

    private void SetFlagCount(LevelData levelData)
    {
        flagCount = levelData.mines;
        ConvertIntToDisplay(flagCount);
    }

    private void UpdateFlagCount(int update)
    {
        flagCount += update;
        ConvertIntToDisplay(flagCount);
    }

    private void ConvertIntToDisplay(int value)
    {
        int hundreds = (int) value / 100;
        int tens = (int) (value % 100) / 10;
        int ones = value % 10;

        bool negative = value < 0;
        hundredsText.gameObject.SetActive(!negative);
        dashText.gameObject.SetActive(negative);

        hundredsText.text = hundreds.ToString();
        tensText.text = tens.ToString();
        onesText.text = ones.ToString();
    }

}
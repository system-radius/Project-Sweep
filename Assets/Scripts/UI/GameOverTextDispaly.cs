using TMPro;
using UnityEngine;

public class GameOverTextDispaly : MonoBehaviour
{
    [SerializeField]
    private string winString = "YOU WIN!";

    [SerializeField]
    private string gameOverString = "GAME OVER!";

    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    private void Awake()
    {
        gameController = gameControllerObject as IGameController;
        if (gameController == null)
        {
            throw new System.Exception("GameController not found!");
        }
        gameOverText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        gameController.OnRestartGame += HideGameOverText;
        gameController.OnStopGame += ShowGameOverText;
        gameController.OnWinGame += SetWinText;
    }

    private void OnDisable()
    {
        gameController.OnRestartGame -= HideGameOverText;
        gameController.OnStopGame -= ShowGameOverText;
        gameController.OnWinGame -= SetWinText;
    }

    private void SetWinText()
    {
        gameOverText.text = winString;
    }

    private void ShowGameOverText()
    {
        gameOverText.gameObject.SetActive(true);
    }

    private void HideGameOverText(LevelData data = null)
    {
        gameOverText.gameObject.SetActive(false);
        gameOverText.text = gameOverString;
    }
}
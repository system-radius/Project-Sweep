using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    [SerializeField]
    private GameObject pausePanelObject;

    [SerializeField]
    private GameObject pauseButton;

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
        gameController.OnResumeGame += HidePausePanel;
        gameController.OnPauseGame += ShowPausePanel;
    }

    private void OnDisable()
    {
        gameController.OnResumeGame -= HidePausePanel;
        gameController.OnPauseGame -= ShowPausePanel;
    }

    private void ShowPausePanel()
    {
        pausePanelObject.SetActive(true);
        pauseButton.SetActive(false);
    }

    private void HidePausePanel()
    {
        pausePanelObject.SetActive(false);
        pauseButton.SetActive(true);
    }
}
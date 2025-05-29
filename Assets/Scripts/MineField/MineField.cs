using System;
using System.Collections;
using UnityEngine;

/*
 * The Mine Field simply keeps track of the cells available to it, and sometimes, will fire events
 * that will affect every cell that it is currently tracking. It also has the capability to alter
 * the game state, give that the cells should not actively handle that part.
 * 
 */
public class MineField : MonoBehaviour
{
    [SerializeField]
    private CameraScaler worldCamera;

    [SerializeField]
    private GameObject cellGameObject;

    [SerializeField]
    private MonoBehaviour gameControllerObject;
    private IGameController gameController;

    public int sizeX = 10;
    public int sizeY = 10;
    public int mines = 10;

    private CellController[,] cells;

    [SerializeField]
    private bool firstTap = true;

    private GameObject cellsParent;

    public Action<int> OnUpdateFlagCount;

    private void Awake()
    {
        //cells = new Cell[sizeX, sizeY];
        gameController = gameControllerObject as IGameController;
        if (gameController == null )
        {
            throw new System.Exception("GameController not found!");
        }
    }

    private void OnEnable()
    {
        gameController.OnTriggerTap += TriggerTap;
        gameController.OnTriggerHold += TriggerHold;
        gameController.OnInitialTrigger += Populate;
        gameController.OnRestartGame += GenerateField;
        gameController.OnPauseGame += HideField;
        gameController.OnResumeGame += ShowField;
    }

    private void OnDisable()
    {
        gameController.OnTriggerTap -= TriggerTap;
        gameController.OnTriggerHold -= TriggerHold;
        gameController.OnInitialTrigger -= Populate;
        gameController.OnRestartGame -= GenerateField;

        gameController.OnPauseGame -= HideField;
        gameController.OnResumeGame -= ShowField;
    }

    private void HideField()
    {
        cellsParent.SetActive(false);
    }

    private void ShowField()
    {
        cellsParent.SetActive(true);
    }

    private void GenerateField(LevelData level)
    {
        sizeX = level.sizeX;
        sizeY = level.sizeY;
        mines = level.mines;
        /*
        worldCamera.transform.position = new Vector3(sizeX / 2 - 0.5f, sizeY / 2 - 0.5f, -10);
        worldCamera.orthographicSize = level.orthographicSize;
        */

        Debug.Log("Generating board: [" + sizeX + ", " + sizeY + "]");

        if (cells != null)
        {
            foreach (CellController cell in cells)
            {
                Destroy(cell.gameObject);
            }
            //return;
            if (cellsParent != null)
            {
                Destroy(cellsParent);
            }
        }

        cellsParent = new GameObject("CellsParent");

        cells = new CellController[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GenerateCell(i, j, cellsParent.transform);
            }
        }

        worldCamera.AdjustCameraSize(cellsParent.transform);
        firstTap = true;
    }

    private void GenerateCell(int x, int y, Transform transform)
    {
        GameObject cell = Instantiate(cellGameObject, transform);
        cell.transform.position = new Vector3(x, y, 0);
        cells[x, y] = cell.GetComponent<CellController>();
        RegisterNeighbors(x, y);
    }

    private void RegisterNeighbors(int x, int y)
    {
        CellController cell = cells[x, y];
        for (int i = -1; i <= 1; i++)
        {
            int checkX = x + i;
            if (checkX < 0 || checkX >= sizeX) continue;
            for (int j = -1; j <= 1; j++)
            {
                int checkY = y + j;
                if (checkY < 0 || checkY >= sizeY) continue;
                CellController neighbor = cells[checkX, checkY];
                if (neighbor == null)continue;

                cell.AddNeighbor(neighbor);
            }
        }
    }

    private bool Populate(Vector3 position)
    {
        if (!firstTap) return true;
        int mines = this.mines;
        CellController originCell = GetCellByPosition(position);
        if (originCell == null) return false;
        while (mines > 0)
        {
            mines--;
            bool minePlaced = false;
            do
            {
                int x = UnityEngine.Random.Range(0, sizeX);
                int y = UnityEngine.Random.Range(0, sizeY);

                CellController cell = cells[x, y];
                if (cell.IsRigged() || (cell == originCell || originCell.IsNeighbor(cell)))
                {
                    continue;
                }

                cell.Rig();
                minePlaced = true;
            } while (!minePlaced);
        }
        firstTap = false;
        return true;
    }

    private bool CheckWinState()
    {
        bool win = true;
        foreach (CellController cell in cells)
        {
            // For a win to be detected, all cells that are not rigged must be revealed.
            win = cell.IsRevealed() || cell.IsRigged();
            if (!win) break;
        }

        if (win) Debug.Log("Win state detected!");
        return win;
    }

    private void RevealAll()
    {
        foreach (CellController cell in cells)
        {
            cell.ForceReveal();
        }
    }

    public int TriggerTap(Vector3 position)
    {
        CellController cell = GetCellByPosition(position);
        if (cell == null) return 0;
        bool flagged = cell.IsFlagged();
        bool continuePlay = cell.TriggerTap();

        if (flagged != cell.IsFlagged())
        {
            OnUpdateFlagCount?.Invoke(flagged ? 1 : -1);
        }

        if (!continuePlay) RevealAll();
        bool win = CheckWinState();
        return continuePlay && !win ? 0 : win ? 1 : -1; ;
    }

    public int TriggerHold(Vector3 position)
    {
        CellController cell = GetCellByPosition(position);
        if (cell == null) return 0;
        bool continuePlay = cell.TriggerHold();
        if (!continuePlay) RevealAll();
        bool win = CheckWinState();
        return continuePlay && !win ? 0 : win ? 1 : -1;
    }

    private CellController GetCellByPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);

        if (x < 0 || x >= sizeX || y < 0 || y >= sizeY) return null;
        return cells[x, y];
    }
}

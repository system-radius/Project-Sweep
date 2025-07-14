using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/*
 * A cell is the building block for the game. It is supposed to track whether it is rigged, as well as its neighbors.
 */
public class CellController : MonoBehaviour
{
    [SerializeField]
    private CellView view;

    public readonly CellModel model = new CellModel();
    private readonly List<CellController> neighbors = new List<CellController>();

    public event Action<bool> OnCellTrigger;
    public event Action<bool> OnCellFlag;
    public event Action<int> OnCellReveal;

    public event Action OnWrongFlag;
    public event Action OnGameOverTrigger;
    public event Action OnRestart;

    private bool holding = false;

    public bool IsNeighbor(CellController that)
    {
        return this.neighbors.Contains(that);
    }

    public void AddNeighbor(CellController that)
    {
        if (this == that) return;
        if (!this.neighbors.Contains(that)) this.neighbors.Add(that);
        if (!that.neighbors.Contains(this)) that.neighbors.Add(this);
    }

    private bool RevealNeighbors(bool forced = false)
    {
        bool state = true;
        foreach (CellController cell in neighbors)
        {
            if (cell.model.IsFlagged()) continue;
            bool currentCellState = cell.Reveal(forced);
            if (state) state = currentCellState;
        }

        return state;
    }

    public void ForceReveal()
    {
        if (model.IsFlagged() && !model.IsRigged()) OnWrongFlag?.Invoke();
        else if (model.IsRigged() && !model.IsFlagged()) Reveal(true);
    }

    public bool Reveal(bool forced = false)
    {
        OnCellTrigger?.Invoke(model.IsRigged());
        if (model.IsRigged())
        {
            if (!forced) OnGameOverTrigger?.Invoke();
            VibrationHelper.Vibrate(500);
            return false;
        }
        if (model.IsRevealed()) return true;

        int value = neighbors.FindAll(c => c.model.IsRigged()).Count;
        model.Reveal(value);
        if (value > 0)
        {
            OnCellReveal?.Invoke(value);
            return true;
        }

        return RevealNeighbors(forced);
    }

    public bool Chord(bool fromTap = true)
    {
        if (model.IsRigged()) return false;

        VibrationHelper.Vibrate(50);
        int flags = neighbors.FindAll(c => c.model.IsFlagged()).Count;
        if (flags != model.Value())
        {
            float duration = fromTap ? 0.1f : -1;
            HighlightNeighbors(true, duration);
            return true;
        }

        return RevealNeighbors();
    }

    private void HighlightNeighbors(bool highlight, float duration = -1f)
    {
        foreach (CellController cell in neighbors)
        {
            if (!cell.model.IsFlagged() && !cell.model.IsRevealed()) StartCoroutine(cell.Highlight(highlight, duration));
        }
    }

    IEnumerator Highlight(bool highlight, float duration)
    {
        if (duration > 0)
        {
            view.HighlightCell(true);
            yield return new WaitForSeconds(duration);
            view.HighlightCell(false);
        }
        else
        {
            view.HighlightCell(highlight);
            yield return null;
        }
    }

    private void OnEnable()
    {
        OnCellTrigger += view.TriggerCell;
        OnCellReveal += view.RevealCell;
        OnCellFlag += view.FlagCell;
        OnWrongFlag += view.SetWrongFlag;
        OnGameOverTrigger += view.SetGameOverTrigger;
    }

    private void OnDisable()
    {
        OnCellTrigger -= view.TriggerCell;
        OnCellReveal -= view.RevealCell;
        OnCellFlag -= view.FlagCell;
        OnWrongFlag -= view.SetWrongFlag;
        OnGameOverTrigger -= view.SetGameOverTrigger;
    }

    public bool TriggerTap()
    {
        if (model.IsRevealed())
        {
            return Chord();
        }
        model.Flag();
        OnCellFlag?.Invoke(model.IsFlagged());
        return true;
    }

    public bool TriggerHold()
    {
        holding = true;
        if (model.IsFlagged()) return true;
        if (model.IsRevealed())
        {
            return Chord(false);
        }

        VibrationHelper.Vibrate(50);
        return Reveal();
    }

    public void StopHold()
    {
        if (holding)
        {
            holding = false;
            HighlightNeighbors(false);
        }
    }
}
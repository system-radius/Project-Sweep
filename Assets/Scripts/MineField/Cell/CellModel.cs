using System;
using System.Collections.Generic;

public class CellModel
{
    private int value = -1;
    private bool rigged = false;
    private bool flagged = false;

    public event Action<bool> OnCellTrigger;
    public event Action<bool> OnCellFlag;
    public event Action<int> OnCellReveal;

    public event Action OnWrongFlag;
    public event Action OnGameOverTrigger;
    public event Action OnRestart;

    private readonly List<CellModel> neighbors = new List<CellModel>();

    public bool IsNeighbor(CellModel that)
    {
        return this.neighbors.Contains(that);
    }

    public void AddNeighbor(CellModel that)
    {
        if (this == that) return;
        if (!this.neighbors.Contains(that)) this.neighbors.Add(that);
        if (!that.neighbors.Contains(this)) that.neighbors.Add(this);
    }

    private bool RevealNeighbors(bool forced = false)
    {
        bool state = true;
        foreach (CellModel cell in neighbors)
        {
            if (cell.IsFlagged()) continue;
            bool currentCellState = cell.Reveal(forced);
            if (state) state = currentCellState;
        }

        return state;
    }

    public void ForceReveal()
    {
        if (IsFlagged() && !IsRigged()) OnWrongFlag?.Invoke();
        else if (IsRigged() && !IsFlagged()) Reveal(true);
    }

    public bool Reveal(bool forced = false)
    {
        OnCellTrigger?.Invoke(IsRigged());
        if (IsRigged())
        {
            if (!forced) OnGameOverTrigger?.Invoke();
            VibrationHelper.Vibrate(500);
            return false;
        }
        if (IsRevealed()) return true;

        value = neighbors.FindAll(c => c.IsRigged()).Count;
        if (value > 0)
        {
            OnCellReveal?.Invoke(value);
            return true;
        }

        return RevealNeighbors(forced);
    }

    public bool Chord()
    {
        if (IsRigged()) return false;

        VibrationHelper.Vibrate(50);
        int flags = neighbors.FindAll(c => c.IsFlagged()).Count;
        if (flags != value) return true;

        return RevealNeighbors();
    }

    public void Rig()
    {
        rigged = true;
    }

    public bool IsRigged()
    {
        return rigged;
    }

    public void Flag()
    {
        flagged = !flagged;
        OnCellFlag?.Invoke(flagged);
    }

    public bool IsFlagged()
    {
        return flagged;
    }

    public bool IsRevealed()
    {
        return value >= 0;
    }
}
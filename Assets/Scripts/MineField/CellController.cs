using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * A cell is the building block for the game. It is supposed to track whether it is rigged, as well as its neighbors.
 */
public class CellController : MonoBehaviour
{
    public event Action<bool> OnCellTrigger;
    public event Action<bool> OnCellFlag;
    public event Action<int> OnCellReveal;

    public event Action OnWrongFlag;
    public event Action OnGameOverTrigger;
    public event Action OnRestart;

    private CellModel model;

    private readonly List<CellController> neighbors = new List<CellController>();

    private void Awake()
    {
        model = new CellModel();
    }

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

        model.value = neighbors.FindAll(c => c.IsRigged()).Count;
        if (model.value > 0)
        {
            OnCellReveal?.Invoke(model.value);
            return true;
        }

        return RevealNeighbors(forced);
    }

    private bool Chord()
    {
        if (IsRigged()) return false;

        VibrationHelper.Vibrate(50);
        int flags = neighbors.FindAll(c => c.IsFlagged()).Count;
        if (flags != model.value) return true;

        return RevealNeighbors();
    }

    public void Rig()
    {
        model.rigged = true;
    }

    public bool IsRigged()
    {
        return model.rigged;
    }

    public void Flag()
    {
        model.flagged = !model.flagged;
    }

    public bool IsFlagged()
    {
        return model.flagged;
    }

    public bool IsRevealed()
    {
        return model.value >= 0;
    }

    public bool TriggerTap()
    {
        if (IsRevealed())
        {
            return Chord();
        }
        Flag();
        OnCellFlag?.Invoke(model.flagged);
        return true;
    }

    public bool TriggerHold()
    {
        if (IsFlagged()) return true;
        if (IsRevealed())
        {
            return Chord();
        }

        VibrationHelper.Vibrate(50);
        return Reveal();
    }
}
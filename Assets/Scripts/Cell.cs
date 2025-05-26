using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * A cell is the building block for the game. It is supposed to track whether it is rigged, as well as its neighbors.
 */
public class Cell : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro tmp;

    public event Action<bool> OnCellTrigger;
    public event Action<bool> OnCellFlag;

    public event Action OnWrongFlag;
    public event Action OnGameOverTrigger;
    public event Action OnRestart;

    private bool rigged = false;
    private bool flagged = false;

    private int value = -1;

    private readonly List<Cell> neighbors = new List<Cell>();

    private Color[] colors =
    {
        new Color(0 / 256f, 0 / 256f, 0 / 256f),
        new Color(2/256f, 2/256f, 241/256f),
        new Color(54 / 256f, 126 / 256f, 26 / 256f),
        new Color(236 / 256f, 51 / 256f, 36 / 256f),
        new Color(1 / 256f, 2 / 256f, 122 / 256f),
        new Color(116 / 256f, 24 / 256f, 12 / 256f),
        new Color(57 / 256f, 124 / 256f, 118 / 256f),
        new Color(1 / 256f, 1 / 256f, 1 / 256f),
        new Color(128 / 256f, 128 / 256f, 128 / 256f)
    };

    public void AddNeighbor(Cell that)
    {
        if (this == that) return;
        if (!this.neighbors.Contains(that)) this.neighbors.Add(that);
        if (!that.neighbors.Contains(this)) that.neighbors.Add(this);
    }

    private bool RevealNeighbors(bool forced = false)
    {
        bool state = true;
        foreach (Cell cell in neighbors)
        {
            if (cell.IsFlagged()) continue;
            bool currentCellState = cell.Reveal(forced);
            if (state) state = currentCellState;
        }

        return state;
    }

    public bool Reveal(bool forced = false)
    {
        OnCellTrigger?.Invoke(IsRigged());
        if (IsRigged())
        {
            if (!forced) OnGameOverTrigger?.Invoke();
            return false;
        }
        if (IsRevealed()) return true;

        value = neighbors.FindAll(c => c.IsRigged()).Count;
        if (value > 0 && value < 9)
        {
            tmp.text = value.ToString();
            tmp.color = colors[value];
            return true;
        }

        return RevealNeighbors(forced);
    }

    private bool Chord()
    {
        if (IsRigged()) return false;

        int flags = neighbors.FindAll(c => c.IsFlagged()).Count;
        if (flags != value) return true;

        return RevealNeighbors();
    }

    public int GetValue()
    {
        return value;
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
    }

    public bool IsFlagged()
    {
        return flagged;
    }

    public bool IsRevealed()
    {
        return value >= 0;
    }

    public bool TriggerTap()
    {
        if (IsRevealed())
        {
            return Chord();
        }
        Flag();
        OnCellFlag?.Invoke(flagged);
        return true;
    }

    public bool TriggerHold()
    {
        if (IsFlagged()) return true;
        if (IsRevealed())
        {
            return Chord();
        }

        return Reveal();
    }

    public void TriggerWrongFlag()
    {
        OnWrongFlag?.Invoke();
    }

    public void Restart()
    {
        value = -1;
        flagged = false;
        rigged = false;
        tmp.text = "";
    }
}
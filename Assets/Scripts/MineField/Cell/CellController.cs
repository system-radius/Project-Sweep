using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * A cell is the building block for the game. It is supposed to track whether it is rigged, as well as its neighbors.
 */
public class CellController : MonoBehaviour
{
    [SerializeField]
    private CellView view;

    public readonly CellModel model = new CellModel();

    private void OnEnable()
    {
        model.OnCellTrigger += view.TriggerCell;
        model.OnCellReveal += view.RevealCell;
        model.OnCellFlag += view.FlagCell;
        model.OnWrongFlag += view.SetWrongFlag;
        model.OnGameOverTrigger += view.SetGameOverTrigger;
    }

    private void OnDisable()
    {
        model.OnCellTrigger -= view.TriggerCell;
        model.OnCellReveal -= view.RevealCell;
        model.OnCellFlag -= view.FlagCell;
        model.OnWrongFlag -= view.SetWrongFlag;
        model.OnGameOverTrigger -= view.SetGameOverTrigger;
    }

    public bool TriggerTap()
    {
        if (model.IsRevealed())
        {
            return model.Chord();
        }
        model.Flag();
        return true;
    }

    public bool TriggerHold()
    {
        if (model.IsFlagged()) return true;
        if (model.IsRevealed())
        {
            return model.Chord();
        }

        VibrationHelper.Vibrate(50);
        return model.Reveal();
    }
}
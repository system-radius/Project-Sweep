using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Cell))]
public class SpriteSelector : MonoBehaviour
{

    public List<Sprite> sprites = new List<Sprite> ();

    public CellDisplayState displayState = CellDisplayState.WAITING;

    private SpriteRenderer spriteRenderer;
    private Cell cell;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        cell = GetComponent<Cell> ();
    }

    private void OnEnable()
    {
        cell.OnCellTrigger += TriggerCell;
        cell.OnCellFlag += FlagCell;
        cell.OnWrongFlag += SetWrongFlag;
        cell.OnGameOverTrigger += SetGameOverTrigger;
    }

    private void OnDisable()
    {
        cell.OnCellTrigger -= TriggerCell;
        cell.OnCellFlag -= FlagCell;
        cell.OnWrongFlag -= SetWrongFlag;
        cell.OnGameOverTrigger -= SetGameOverTrigger;
    }

    public void TriggerCell(bool state)
    {
        ChangeDisplayState(state ? CellDisplayState.TRIGGERED : CellDisplayState.REVEALED);
    }

    public void FlagCell(bool state)
    {
        ChangeDisplayState(state ? CellDisplayState.FLAGGED : CellDisplayState.WAITING);
    }

    public void ChangeDisplayState(CellDisplayState displayState)
    {
        this.displayState = displayState;
        spriteRenderer.sprite = sprites[(int)displayState];
    }

    public void SetGameOverTrigger()
    {
        spriteRenderer.color = Color.red;
    }

    public void SetWrongFlag()
    {
        spriteRenderer.color = Color.yellow;
    }
}


public enum CellDisplayState
{
    REVEALED,
    WAITING,
    FLAGGED,
    TRIGGERED,
}
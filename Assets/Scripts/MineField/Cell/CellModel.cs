using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellModel
{
    private int value = -1;
    private bool rigged = false;
    private bool flagged = false;

    public void Reveal(int value)
    {
        this.value = value;
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

    public int Value()
    {
        return value;
    }
}
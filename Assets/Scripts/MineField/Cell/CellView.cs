using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class CellView : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro tmp;

    public List<Sprite> sprites = new List<Sprite> ();

    public CellDisplayState displayState = CellDisplayState.WAITING;

    private SpriteRenderer spriteRenderer;

    private static Color[] colors =
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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    public void RevealCell(int value)
    {
        if (value > 0 && value < colors.Length)
        {
            tmp.text = value.ToString();
            tmp.color = colors[value];
        }
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
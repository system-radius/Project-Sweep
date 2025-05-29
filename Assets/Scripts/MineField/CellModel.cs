public class CellModel
{
    public int x;
    public int y;

    public int value = -1;
    public bool rigged = false;
    public bool flagged = false;

    // Cell neighbors are to be tracked by the controller,
    // as it will reference other cell controllers mainly
    // for the chord and reveal functionalities.
}
namespace Lab4Core.GameObjects;

public abstract class Cell
{
    private readonly int _x;
    private readonly int _y;
    public (int, int) GetPosition() => (_x, _y);
    public abstract int GetColorId();
    protected Cell(int x, int y)
    {
        _x = x;
        _y = y;
    }
    public bool Equals(Cell other) => GetColorId() == other.GetColorId() && GetPosition() == other.GetPosition();
}

public class FoodCell: Cell
{
    public FoodCell(int x, int y) : base(x, y) { }
    public override int GetColorId() => 1;
}

public class EmptyCell: Cell
{
    public EmptyCell(int x, int y) : base(x, y) { }
    public override int GetColorId() => 0;
}

public class SnakeCell: Cell
{
    public SnakeCell(int x, int y, int colorId) : base(x, y) { _colorId = colorId; }
    private readonly int _colorId;
    public override int GetColorId() => _colorId;
}
namespace Lab4Core.GameObjects;

public class GameField
{
    private readonly List<List<Cell>> _field;

    //initialize height*width 2D List with EmptyCell
    public GameField(int width, int height)
    {
        _field = Enumerable.Range(0, height)
            .Select(y => Enumerable.Range(0, width)
                .Select(x => (Cell)new EmptyCell(x, y))
                .ToList())
            .ToList();
    }

    public Cell GetCell(int x, int y)
    {
        return _field[y][x];
    }

    public void SetCell(Cell cell)
    {
        int tempX = cell.GetPosition().Item1;
        int tempY = cell.GetPosition().Item2;
        _field[tempY][tempX] = cell;
    }
    
    public int GetWidth()
    {
        return _field[0].Count;
    }

    public int GetHeight()
    {
        return _field.Count;
    }
}
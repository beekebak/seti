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

    public List<(int x, int y)> GetFoodPositions()
    {
        List<(int x, int y)> positions = new List<(int x, int y)>();
        for (int i = 0; i < _field.Count; i++)
        {
            for (int j = 0; j < _field[i].Count; j++)
            {
                if(_field[i][j] is FoodCell cell) positions.Add(cell.GetPosition());
            }
        }
        return positions;
    }
}
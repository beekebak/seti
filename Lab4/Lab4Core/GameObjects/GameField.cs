using Lab4Core.Abstractions;

namespace Lab4Core.GameObjects;

public class GameField
{
    private readonly List<List<ICell>> _field;

    public GameField(int width, int height)
    {
        _field = new List<List<ICell>>(height);
        for (int y = 0; y < height; y++)
        {
            var tempList = new List<ICell>(width);
            for (int x = 0; x < width; x++)
            {
                tempList.Add(new EmptyCell(x, y));
            }
            _field.Add(tempList);
        }
    }

    public ICell GetCell(int x, int y)
    {
        return _field[y][x];
    }

    public void SetCell(ICell cell)
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
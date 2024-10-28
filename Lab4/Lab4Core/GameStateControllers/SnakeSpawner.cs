using Lab4Core.GameObjects;
using static Lab4Core.Utility;

namespace Lab4Core.GameStateControllers;

class SnakeSpawner
{
    private void SetSquareUnavailable(int x, int y, GameField field, bool[,] fieldCopy)
    {
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                fieldCopy[GetNewCoord(y, i, field.GetHeight()), GetNewCoord(x, j, field.GetWidth())] = true;
            }
        }
    }

    private bool EverythingAroundIsFood(int x, int y, GameField field)
    {
        List<(int, int)> cells = [(x, GetNewCoord(y, 1, field.GetHeight())),
            (x, GetNewCoord(y, -1, field.GetHeight())),
            (GetNewCoord(x, 1, field.GetWidth()), y),
            (GetNewCoord(x, -1, field.GetWidth()), y)];
        return cells.All(cell => field.GetCell(cell.Item1, cell.Item2) is FoodCell);
    }
    
    private void CheckUnavailability(GameField field, bool[,] fieldCopy)
    {
        for(int y = 0; y < field.GetHeight(); y++)
        {
            for (int x = 0; x < field.GetWidth(); x++)
            {
                if(field.GetCell(x, y) is SnakeCell) SetSquareUnavailable(x, y, field, fieldCopy);
                else if(field.GetCell(x, y) is FoodCell) fieldCopy[y, x] = true;
                else if(EverythingAroundIsFood(x, y, field)) fieldCopy[y, x] = true;
            }
        }
    }

    private Snake MakeNewSnake((int, int) coords, GameField field)
    {
        int x = coords.Item1;
        int y = coords.Item2;
        var random = new Random();
        var directions = Enum.GetValues(typeof(Directions))
            .Cast<Directions>()
            .OrderBy(_ => random.Next());
        foreach (var direction in directions)
        {
            int fixedX, fixedY;
            switch (direction)
            {
                case Directions.Up:
                    fixedY = GetNewCoord(y, 1, field.GetHeight());
                    if (field.GetCell(x, fixedY) is not FoodCell)
                        return new Snake([(x, y), (x, fixedY)], random.Next(), Directions.Down);
                    break;
                case Directions.Down:
                    fixedY = GetNewCoord(y, -1, field.GetHeight());
                    if (field.GetCell(x, fixedY) is not FoodCell)
                        return new Snake([(x, y), (x, fixedY)], random.Next(), Directions.Up);
                    break;
                case Directions.Left:
                    fixedX = GetNewCoord(y, -1, field.GetWidth());
                    if (field.GetCell(fixedX, y) is not FoodCell)
                        return new Snake([(x, y), (fixedX, y)], random.Next(), Directions.Right);
                    break;
                case Directions.Right:
                    fixedX = GetNewCoord(y, 1, field.GetWidth());
                    if (field.GetCell(fixedX, y) is not FoodCell)
                        return new Snake([(x, y), (fixedX, y)], random.Next(), Directions.Left);
                    break;
            }
        }
        // should never happen
        throw new Exception("Couldn't make snake because food everywhere");
    }

    private List<Snake> GetNewSnakes(GameField field, bool[,] fieldCopy, int count)
    {
        var random = new Random();
        IEnumerable<(int, int)> falseCells = 
            Enumerable.Range(0, fieldCopy.GetLength(0))
                .SelectMany(i => Enumerable.Range(0, fieldCopy.GetLength(1))
                    .Where(j => !fieldCopy[i, j])
                    .Select(j => (j, i))
                    .OrderBy(_ => random.Next()));
        return falseCells.Select(coords => MakeNewSnake(coords, field)).Take(count).ToList();
    }
    
    public List<Snake> Spawn(int newSnakesCount, GameField field)
    {
            bool[,] fieldCopy = new bool[field.GetHeight(), field.GetWidth()];
            CheckUnavailability(field, fieldCopy);
            return GetNewSnakes(field, fieldCopy, newSnakesCount);
    }
}
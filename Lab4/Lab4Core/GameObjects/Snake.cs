namespace Lab4Core.GameObjects;

public class Snake
{
    public IList<SnakeCell> Body { get; }
    public Directions Direction { get; set; }
    public int PlayerId { get; }

    public Snake(IList<SnakeCell> initBody, int color, Directions direction = Directions.Left)
    {
        Body = initBody;
        PlayerId = color;
        Direction = direction;
    }

    public Snake(List<(int x, int y)> initBody, int id, Directions direction = Directions.Left)
    {
        Body = new List<SnakeCell>();
        foreach (var coord in initBody)
        {
            Body.Add(new SnakeCell(coord.x, coord.y, id));
        }
        PlayerId = id;
        Direction = direction;
    }

    public SnakeCell GetHead()
    {
        return Body[0];
    }

    public bool ContainsCell(Cell cell)
    {
        return Body.Any(snakeCell => snakeCell.GetPosition() == cell.GetPosition());
    }

    public Directions GetForbiddenDirection()
    {
        int dx = Body[0].GetPosition().Item1 - Body[1].GetPosition().Item1;
        int dy = Body[0].GetPosition().Item2 - Body[1].GetPosition().Item2;
        if (dx == -1 || dx > 1)
        {
            return Directions.Right;
        } 
        if (dy == -1 || dy > 1)
        {
            return Directions.Up;
        }
        if (dx == 1 || dx < -1)
        {
            return Directions.Left;
        }

        if (dy == 1 || dy < -1)
        {
            return Directions.Down;
        }
        throw new Exception("Invalid snake body values");
    }
}
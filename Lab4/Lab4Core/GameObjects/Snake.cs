namespace Lab4Core.GameObjects;

public class Snake
{
    public IList<SnakeCell> Body { get; }
    public Directions Direction { get; set; }
    public int Color { get; }

    public Snake(IList<SnakeCell> initBody, int color, Directions direction = Directions.Left)
    {
        Body = initBody;
        Color = color;
        Direction = direction;
    }

    public Snake(List<(int x, int y)> initBody, int color, Directions direction = Directions.Left)
    {
        Body = new List<SnakeCell>();
        foreach (var coord in initBody)
        {
            Body.Add(new SnakeCell(coord.x, coord.y, color));
        }
        Color = color;
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
}
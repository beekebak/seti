using Lab4Core.Abstractions;

namespace Lab4Core.GameObjects;

public class Snake
{
    public List<SnakeCell> Body { get; set; }
    public Directions Direction { get; set; } = Directions.Left;
    public int Color { get; }

    public Snake(List<SnakeCell> initBody, int color)
    {
        Body = initBody;
        Color = color;
    }

    public Snake(List<(int x, int y)> initBody, int color)
    {
        Body = new List<SnakeCell>();
        foreach (var coord in initBody)
        {
            Body.Add(new SnakeCell(coord.x, coord.y, color));
        }
        Color = color;
    }

    public SnakeCell GetHead()
    {
        return Body[0];
    }
}
using Lab4Core;
using Snakes;

namespace Lab4.NetworkService;

public class Converter
{
    public Directions GetDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Directions.Up;
            case Direction.Down: return Directions.Down;
            case Direction.Left: return Directions.Left;
            case Direction.Right: return Directions.Right;
        }
        throw new Exception("Invalid Direction DTO");
    }

    public Direction GetDirectionDto(Directions dir)
    {
        switch (dir)
        {
            case Directions.Up: return Direction.Up;
            case Directions.Down: return Direction.Down;
            case Directions.Left: return Direction.Left;
            case Directions.Right: return Direction.Right;
        }
        throw new Exception("Invalid Direction");
    }
}
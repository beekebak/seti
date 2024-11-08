using Lab4Core;
using Snakes;

namespace Lab4.NetworkService;

public static class Converter
{
    public static Directions GetDirection(this Direction dir)
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

    public static Direction GetDirectionDto(this Directions dir)
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
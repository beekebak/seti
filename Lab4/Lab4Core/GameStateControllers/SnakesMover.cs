using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using static Lab4Core.Utility;

namespace Lab4Core.GameStateControllers;

class SnakesMover
{
    private SnakeCell GetNewHead(Snake snake, GameField field)
    {
        var moveDirection = snake.Direction;
        SnakeCell newCell = null!;
        (int oldX, int oldY) = snake.GetHead().GetPosition();
        switch (moveDirection)
        {
            case Directions.Left:
                newCell = new SnakeCell(getNewCoord(oldX, -1, field.GetWidth()),
                    getNewCoord(oldY, 0, field.GetHeight()), snake.Color);
                break;
            case Directions.Down:
                newCell = new SnakeCell(getNewCoord(oldX, 0, field.GetWidth()),
                    getNewCoord(oldY, -1, field.GetHeight()), snake.Color);
                break;
            case Directions.Right:
                newCell = new SnakeCell(getNewCoord(oldX, 1, field.GetWidth()),
                    getNewCoord(oldY, 0, field.GetHeight()), snake.Color);
                break;
            case Directions.Up:
                newCell = new SnakeCell(getNewCoord(oldX, 0, field.GetWidth()),
                    getNewCoord(oldY, 1, field.GetHeight()), snake.Color);
                break;
        }
        return newCell;
    }

    private void FixTail(Snake snake, GameField field, SnakeCell newCell)
    {
        if (field.GetCell(newCell.GetPosition().Item1, newCell.GetPosition().Item2) is not FoodCell)
        {   
            snake.Body.RemoveAt(snake.Body.Count - 1);
        }
    }
    
    public void MoveSnakes(List<Snake> snakes, GameField field)
    {
        foreach (var snake in snakes)
        {
            SnakeCell newCell = GetNewHead(snake, field);
            snake.Body.Insert(0, newCell);
            FixTail(snake, field, newCell);
        }
    }
}
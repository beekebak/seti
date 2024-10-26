using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using Lab4Core.GameStateControllers;

namespace Lab4Tests.GameFieldTests;

public class SnakeMovementTests
{
    [Fact]
    public void SnakeMovement_MoveSnake_Test()
    {
        var tempSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (1,3)};
        Snake snake = new Snake(tempSnakeBody, 1);
        GameField field = new GameField(5, 5);
        GameStateUpdater updater = new GameStateUpdater([snake], field);
            
        updater.Update();
        
        var expected = new List<(int x, int y)>{(0,1), (1,1), (1,2)};
        for (int i = 0; i < snake.Body.Count; i++)
        {
            Assert.Equivalent(expected[i], snake.Body[i].GetPosition());
        }
    }

    [Fact]
    public void SnakeMovement_MoveSnakeThroughBorder_Test()
    {
        var tempSnakeBody = new List<(int x, int y)>{(1,0), (1,1), (1,2)};
        Snake snake = new Snake(tempSnakeBody, 1);
        snake.Direction = Directions.Down;
        GameField field = new GameField(3, 6);
        GameStateUpdater updater = new GameStateUpdater([snake], field);
            
        for(int i = 0; i < 3; i++) updater.Update();
        
        var expected = new List<(int x, int y)>{(1,3), (1,4), (1,5)};
        for (int i = 0; i < snake.Body.Count; i++)
        {
            Assert.Equivalent(expected[i], snake.Body[i].GetPosition());
        }
    }
}
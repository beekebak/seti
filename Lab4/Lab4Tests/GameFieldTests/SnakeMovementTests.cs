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
        Snake snake = new Snake(tempSnakeBody, 1, Directions.Left);
        GameField field = new GameField(5, 5);
        GameStateUpdater updater = new GameStateUpdater([snake], field, new Mock<ScoreBoard>().Object);
            
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
        Snake snake = new Snake(tempSnakeBody, 1, Directions.Down);
        GameField field = new GameField(3, 6);
        GameStateUpdater updater = new GameStateUpdater([snake], field, new Mock<ScoreBoard>().Object);
            
        for(int i = 0; i < 3; i++) updater.Update();
        
        var expected = new List<(int x, int y)>{(1,3), (1,4), (1,5)};
        for (int i = 0; i < snake.Body.Count; i++)
        {
            Assert.Equivalent(expected[i], snake.Body[i].GetPosition());
        }
    }

    [Fact]
    public void SnakeMovement_EatsFood_Test()
    {
        var tempSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (1,3)};
        Snake snake = new Snake(tempSnakeBody, 1, Directions.Right);
        GameField field = new GameField(5, 5);
        field.SetCell(new FoodCell(2, 1));
        GameStateUpdater updater = new GameStateUpdater([snake], field, new Mock<ScoreBoard>().Object);
            
        updater.Update();
        
        var expected = new List<(int x, int y)>{(2,1), (1,1), (1,2), (1,3)};
        for (int i = 0; i < snake.Body.Count; i++)
        {
            Assert.Equivalent(expected[i], snake.Body[i].GetPosition());
        }
    }
}
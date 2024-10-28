using Lab4Core;
using Lab4Core.GameObjects;
using Lab4Core.GameStateControllers;

namespace Lab4Tests.GameFieldTests;

public class GameFieldUpdateTests
{
    [Fact]
    public void GameFieldUpdate_GameFieldContainsAliveSnakes_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (1,3)};
        Snake snakeCollided = new Snake(collidedSnakeBody, 1, Directions.Right);
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake>{snakeCollided};
        GameStateUpdater updater = new GameStateUpdater(snakes, field, new ScoreBoard());
            
        updater.Update();
        
        Assert.Equivalent(new SnakeCell(1, 1, 1), field.GetCell(1, 1));
        Assert.Equivalent(new SnakeCell(1, 2, 1), field.GetCell(1, 2));
        Assert.Equivalent(new SnakeCell(2, 1, 1), field.GetCell(2, 1));
    }

    [Fact]
    public void GameFieldUpdate_GameFieldDoesNotContainsOldSnakes_Test()
    {
        GameField field = new GameField(5, 5);
        field.SetCell(new SnakeCell(3, 3, 3));
        GameStateUpdater updater = new GameStateUpdater(new List<Snake>(), field, new ScoreBoard());
        
        updater.Update();
        
        Assert.IsNotType<SnakeCell>(field.GetCell(3, 3));
    }

    [Fact]
    public void GameFieldUpdate_DeadSnakeExplodes_Test()
    {
        GameField field = new GameField(5, 5);
        var collidedSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (2,2), (2,1), (2, 0)};
        Snake snake = new Snake(collidedSnakeBody, 1, Directions.Right);
        GameStateUpdater updater = new GameStateUpdater([snake], field, new ScoreBoard());
            
        updater.Update();
        
        Assert.All(collidedSnakeBody, coords =>
            Assert.True(field.GetCell(coords.x, coords.y) is EmptyCell or FoodCell));
    }

    [Fact]
    public void GameFieldUpdate_AtLeastMinimumFoodOnField_Test()
    {
        GameField field = new GameField(5, 10);
        GameStateUpdater updater = new GameStateUpdater(new List<Snake>(), field, new ScoreBoard(), 5);
        
        updater.Update();

        int foodCount = 0;
        for(var y = 0; y < field.GetHeight(); y++)
        {
            for (var x = 0; x < field.GetWidth(); x++)
            {
                foodCount += field.GetCell(x, y) is FoodCell ? 1 : 0;
            }
        }
        Assert.Equal(5, foodCount);
    }
    
    [Fact]
    public void GameFieldUpdate_AtLeastMinimumFoodOnFieldIfSnakeDies_Test()
    {
        GameField field = new GameField(5, 10);
        var collidedSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (2,2), (2,1), (2, 0)};
        Snake snake = new Snake(collidedSnakeBody, 1, Directions.Right);
        GameStateUpdater updater = new GameStateUpdater([snake], field, new ScoreBoard(), 7);
        
        updater.Update();

        int foodCount = 0;
        for(var y = 0; y < field.GetHeight(); y++)
        {
            for (var x = 0; x < field.GetWidth(); x++)
            {
                foodCount += field.GetCell(x, y) is FoodCell ? 1 : 0;
            }
        }
        Assert.Equal(7, foodCount);
    }
}
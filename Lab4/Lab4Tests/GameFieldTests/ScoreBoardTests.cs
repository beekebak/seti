using Lab4Core;
using Lab4Core.GameObjects;
using Lab4Core.GameStateControllers;

namespace Lab4Tests.GameFieldTests;

public class ScoreBoardTests
{
    [Fact]
    public void ScoreBoard_FoodGivesScore_Test()
    {
        var tempSnakeBody = new List<(int x, int y)> { (1, 1), (1, 2), (1, 3) };
        Snake snake = new Snake(tempSnakeBody, 1, Directions.Right);
        GameField field = new GameField(5, 5);
        field.SetCell(new FoodCell(2, 1));
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater([snake], field, scoreBoard);

        updater.Update();

        Assert.Equal(1, scoreBoard.GetScore(snake));
    }

    [Fact]
    public void ScoreBoard_KillGivesScore_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (1,3)};
        var colliderSnakeBody = new List<(int x, int y)>{(1,0), (2,0), (3,0), (4,0) };
        Snake snakeCollided = new Snake(collidedSnakeBody, 1, Directions.Right);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Up); 
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater(snakes, field, scoreBoard);
            
        updater.Update();
        
        Assert.Equal(1, scoreBoard.GetScore(snakeCollided));
    }
    
    [Fact]
    public void ScoreBoard_SnakeHasZeroScoreByDefault_Test()
    {
        var colliderSnakeBody = new List<(int x, int y)>{(2,2), (2,1), (2,0)};
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Left); 
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake>{snakeCollider};
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater(snakes, field, scoreBoard);
            
        updater.Update();
        
        Assert.Equal(0, scoreBoard.GetScore(snakeCollider));
    }
    
    [Fact]
    public void ScoreBoard_SelfKillDoesNotGiveScore_Test()
    {
        var colliderSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (2,2), (2,1), (2,0)};
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Right); 
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake>{snakeCollider};
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater(snakes, field, scoreBoard);
            
        updater.Update();
        
        Assert.Equal(0, scoreBoard.GetScore(snakeCollider));
    }

    [Fact]
    public void ScoreBoard_GetsKillScoreEvenIfRemoved_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(1,0), (2,0), (3,0)};
        var colliderSnakeBody = new List<(int x, int y)>{(0,1), (0,2), (0,3)};
        Snake snakeCollided = new Snake(collidedSnakeBody, 1, Directions.Left);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Down); 
        GameField field = new GameField(6, 5);
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater(snakes, field, scoreBoard);
            
        updater.Update();
        
        Assert.Equal(1, scoreBoard.GetScore(snakeCollider));
        Assert.Equal(1, scoreBoard.GetScore(snakeCollided));
    }
    
    [Fact]
    public void ScoreBoard_GetScoresByBothFoodAndKill_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(1,0), (2,0), (3,0)};
        var colliderSnakeBody = new List<(int x, int y)>{(0,1), (0,2), (0,3)};
        Snake snakeCollided = new Snake(collidedSnakeBody, 1, Directions.Left);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Down); 
        GameField field = new GameField(6, 5);
        field.SetCell(new FoodCell(0, 0));
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        ScoreBoard scoreBoard = new ScoreBoard();
        GameStateUpdater updater = new GameStateUpdater(snakes, field, scoreBoard);
            
        updater.Update();
        
        Assert.Equal(2, scoreBoard.GetScore(snakeCollider));
        Assert.Equal(2, scoreBoard.GetScore(snakeCollided));
    }
}
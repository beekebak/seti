using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using Lab4Core.GameStateControllers;

namespace Lab4Tests.GameFieldTests;

public class SnakesCollisionsTests
{
    [Fact]
    public void SnakeCollisions_CollideOneSnakeToAnotherBody_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(1,1), (1,2), (1,3)};
        var colliderSnakeBody = new List<(int x, int y)>{(1,0), (2,0), (3,0), (4,0) };
        Snake snakeCollided = new Snake(collidedSnakeBody, 1, Directions.Right);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2, Directions.Up); 
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        GameStateUpdater updater = new GameStateUpdater(snakes, field);
            
        updater.Update();
        
        Assert.Single(snakes);
        Assert.Equal(snakes[0], snakeCollided);
    }

    [Fact]
    public void SnakeCollisions_CanCollideItself_Test()
    {
        var snakeBody = new List<(int x, int y)>{(1,1), (1,2), (2,2), (2,1), (2,0)};
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake> {new Snake(snakeBody, 1, Directions.Right)};
        GameStateUpdater updater = new GameStateUpdater(snakes, field);
        
        updater.Update();
        
        Assert.Empty(snakes);
    }

    [Fact]
    public void SnakeCollisions_CanChase_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(0,0), (1,0), (2,0)};
        var colliderSnakeBody = new List<(int x, int y)>{(3,0), (4,0), (5,0)};
        Snake snakeCollided = new Snake(collidedSnakeBody, 1);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2); 
        GameField field = new GameField(6, 5);
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        GameStateUpdater updater = new GameStateUpdater(snakes, field);
            
        updater.Update();
        
        Assert.Equal(2, snakes.Count);
    }

    [Fact]
    public void SnakeCollisions_CanChaseSelf_Test()
    {
        var snakeBody = new List<(int x, int y)>{(1,1), (1,2), (2,2), (2,1)};
        GameField field = new GameField(5, 5);
        var snakes = new List<Snake> {new Snake(snakeBody, 1, Directions.Right)};
        GameStateUpdater updater = new GameStateUpdater(snakes, field);
        
        updater.Update();
        
        Assert.Single(snakes);
    }

    [Fact]
    public void SnakeCollisions_DoesNotCollideWhenShouldNot_Test()
    {
        var collidedSnakeBody = new List<(int x, int y)>{(0,0), (1,0), (2,0)};
        var colliderSnakeBody = new List<(int x, int y)>{(0,3), (1,3), (2,3)};
        Snake snakeCollided = new Snake(collidedSnakeBody, 1);
        Snake snakeCollider = new Snake(colliderSnakeBody, 2); 
        GameField field = new GameField(6, 5);
        var snakes = new List<Snake>{snakeCollided, snakeCollider};
        GameStateUpdater updater = new GameStateUpdater(snakes, field);
            
        updater.Update();
        
        Assert.Equal(2, snakes.Count);
    }
}
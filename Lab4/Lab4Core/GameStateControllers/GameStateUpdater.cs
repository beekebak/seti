using System.Collections.Concurrent;
using Lab4Core.GameObjects;

namespace Lab4Core.GameStateControllers;

public class GameStateUpdater
{
    private readonly MyConcurrentList<Snake> _snakes;
    private readonly GameField _gameField;
    private readonly ScoreBoard _scoreBoard;
    private readonly int _foodStatic;
    private readonly SnakesMover _snakesMover = new();
    private readonly SnakeCollider _snakeCollider = new();
    private readonly ScoreBoardUpdater _scoreBoardUpdater = new();
    private readonly GameFieldUpdater _gameFieldUpdater = new();
    private readonly SnakeSpawner _snakeSpawner = new();
    
    public GameStateUpdater(MyConcurrentList<Snake> snakes, GameField gameField,
                            ScoreBoard scoreBoard, int foodStatic = 0)
    {
        _snakes = snakes;
        _gameField = gameField;
        _scoreBoard = scoreBoard;
        _foodStatic = foodStatic;
    }

    private int FixFoodCount()
    {
        return _foodStatic + _snakes.Count;
    }

    public Snake? SpawnSnake(int id)
    {
        var newSnake = _snakeSpawner.Spawn(_gameField, id);
        if (newSnake == null) return newSnake;
        _gameFieldUpdater.AddNewSnakes([newSnake], _gameField);
        _snakes.Add(newSnake);
        return newSnake;
    }
    
    public void Update()
    {
        _snakesMover.MoveSnakes(_snakes, _gameField);
        _snakeCollider.CollideSnakes(_snakes, out var removedSnakes, _gameField.GetWidth(), _gameField.GetHeight());
        _scoreBoardUpdater.UpdateScore(_snakes, removedSnakes, _gameField, _scoreBoard);
        _gameFieldUpdater.UpdateGameField(_snakes, removedSnakes, _gameField, FixFoodCount());
    }
}
using Lab4Core.GameObjects;

namespace Lab4Core.GameStateControllers;

public class GameStateUpdater
{
    private readonly List<Snake> _snakes;
    private readonly GameField _gameField;
    private readonly ScoreBoard _scoreBoard;
    private readonly int _foodStatic;
    private readonly SnakesMover _snakesMover = new();
    private readonly SnakeCollider _snakeCollider = new();
    private readonly ScoreBoardUpdater _scoreBoardUpdater = new();
    private readonly GameFieldUpdater _gameFieldUpdater = new();
    private readonly SnakeSpawner _snakeSpawner = new();
    
    public GameStateUpdater(List<Snake> snakes, GameField gameField,
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
    
    public void Update(int newSnakesCount = 0)
    {
        _snakesMover.MoveSnakes(_snakes, _gameField);
        _snakeCollider.CollideSnakes(_snakes, out var removedSnakes, _gameField.GetWidth(), _gameField.GetHeight());
        _scoreBoardUpdater.UpdateScore(_snakes, removedSnakes, _gameField, _scoreBoard);
        _gameFieldUpdater.UpdateGameField(_snakes, removedSnakes, _gameField, FixFoodCount());
        var newSnakes = _snakeSpawner.Spawn(newSnakesCount, _gameField);
        _gameFieldUpdater.AddNewSnakes(newSnakes, _gameField);
    }
}
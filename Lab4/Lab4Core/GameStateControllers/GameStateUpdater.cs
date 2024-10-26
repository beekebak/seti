using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using static Lab4Core.Utility;

namespace Lab4Core.GameStateControllers;

public class GameStateUpdater : IGameStateUpdater
{
    private readonly List<Snake> _snakes;
    private readonly GameField _gameField;
    private readonly ScoreBoard _scoreBoard;
    private readonly SnakesMover _snakesMover = new();
    private readonly SnakeCollider _snakeCollider = new();
    private readonly ScoreBoardUpdater _scoreBoardUpdater = new();
    
    public GameStateUpdater(List<Snake> snakes, GameField gameField, ScoreBoard scoreBoard)
    {
        _snakes = snakes;
        _gameField = gameField;
        _scoreBoard = scoreBoard;
    }

    public void Update()
    {
        HashSet<Snake> removedSnakes;
        _snakesMover.MoveSnakes(_snakes, _gameField);
        _snakeCollider.CollideSnakes(_snakes, out removedSnakes, _gameField.GetWidth(), _gameField.GetHeight());
        _scoreBoardUpdater.UpdateScore(_snakes, removedSnakes, _gameField, _scoreBoard);
    }
}
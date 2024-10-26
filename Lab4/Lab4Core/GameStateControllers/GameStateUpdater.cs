using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using static Lab4Core.Utility;

namespace Lab4Core.GameStateControllers;

public class GameStateUpdater : IGameStateUpdater
{
    private List<Snake> _snakes;
    private readonly GameField _gameField;
    private readonly SnakesMover _snakesMover = new();
    private readonly SnakeCollider _snakeCollider = new();
    
    public GameStateUpdater(List<Snake> snakes, GameField gameField)
    {
        _snakes = snakes;
        _gameField = gameField;
    }

    public void Update()
    {
        _snakesMover.MoveSnakes(_snakes, _gameField);
        _snakeCollider.CollideSnakes(_snakes, _gameField.GetWidth(), _gameField.GetHeight());
    }
}
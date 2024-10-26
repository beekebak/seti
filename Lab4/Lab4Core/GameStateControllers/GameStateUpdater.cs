using Lab4Core.Abstractions;
using Lab4Core.GameObjects;
using static Lab4Core.Utility;

namespace Lab4Core.GameStateControllers;

public class GameStateUpdater : IGameStateUpdater
{
    private readonly List<Snake> _snakes;
    private readonly GameField _gameField;
    private readonly SnakesMover _snakesMover = new();
    
    public GameStateUpdater(List<Snake> snakes, GameField gameField)
    {
        _snakes = snakes;
        _gameField = gameField;
    }

    public void Update()
    {
        _snakesMover.MoveSnakes(_snakes, _gameField);
    }
}
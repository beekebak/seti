using Lab4Core.GameStateControllers;
using Lab4Core.GameObjects;
namespace Lab4Core;

public class GameContext
{
    private GameField _field;
    private IList<Snake> _snakes;
    private int _foodCount;
    private GameStateUpdater _gameStateUpdater;
    private ScoreBoard _scoreBoard;
}
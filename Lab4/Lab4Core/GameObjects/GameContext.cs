using Lab4Core.Abstractions;
namespace Lab4Core.GameObjects;

public class GameContext
{
    private GameField _field;
    private IList<Snake> _snakes;
    private int _foodCount;
    private IGameStateUpdater _gameStateUpdater;
}
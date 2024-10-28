using Lab4Core.GameStateControllers;
using Lab4Core.GameObjects;
namespace Lab4Core;

public class GameContext
{
    private GameField _field;
    public GameField Field => _field;
    private List<Snake> _snakes;
    private int _foodCount;
    private GameStateUpdater _gameStateUpdater;
    private ScoreBoard _scoreBoard;
    public ScoreBoard ScoreBoard => _scoreBoard;

    public GameContext()
    {
        _field = new GameField(0, 0);
        _snakes = new List<Snake>();
        _foodCount = 5;
        _scoreBoard = new ScoreBoard();
        _gameStateUpdater = new GameStateUpdater(_snakes, _field, _scoreBoard, _foodCount);
    }

    public void InitContext(int width, int height)
    {
        _field = new GameField(width, height);
        _snakes = new List<Snake>();
        _scoreBoard = new ScoreBoard();
        _gameStateUpdater = new GameStateUpdater(_snakes, _field, _scoreBoard, _foodCount);
    }

    public void UpdateGameState(int snakeCount = 0)
    {
        _gameStateUpdater.Update(snakeCount);
    }
}
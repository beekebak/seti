using Lab4Core.GameStateControllers;
using Lab4Core.GameObjects;

namespace Lab4Core;

public class GameContext
{
    public GameField Field { get; private set; }
    public List<Snake> Snakes { get; private set; }
    public int FoodCount { get; }
    public ScoreBoard ScoreBoard { get; private set; }
    public int Delay { get; }
    private GameStateUpdater _gameStateUpdater;
    public int StateOrder { get; private set; }

    public GameContext(int width, int height, int foodCount, int delay)
    {
        Field = new GameField(width, height);
        Snakes = new List<Snake>();
        FoodCount = foodCount;
        ScoreBoard = new ScoreBoard();
        Delay = delay;
        _gameStateUpdater = new GameStateUpdater(Snakes, Field, ScoreBoard, FoodCount);
    }

    public void UpdateGameContext(GameField field, List<Snake> snakes, ScoreBoard scores)
    {
        Field = field;
        Snakes = snakes;
        ScoreBoard = scores;
    }
    
    public void Play()
    {
        _gameStateUpdater.Update();
        StateOrder++;
    }

    public Snake? GetNewSnake(int id) => _gameStateUpdater.SpawnSnake(id);
}
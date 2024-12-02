using System.Collections.Concurrent;
using Lab4Core.GameStateControllers;
using Lab4Core.GameObjects;

namespace Lab4Core;

public class GameContext
{
    public GameField Field { get; private set; }
    public MyConcurrentList<Snake> Snakes { get; private set; }
    public int FoodCount { get; }
    public ScoreBoard ScoreBoard { get; private set; }
    public int Delay { get; }
    public int StateOrder { get; private set; }
    private GameStateUpdater _gameStateUpdater;
    
    public GameContext(int width, int height, int foodCount, int delay, List<Snake>? snakes = null)
    {
        Field = new GameField(width, height);
        Snakes = snakes != null ? new MyConcurrentList<Snake>(snakes) : new MyConcurrentList<Snake>();
        FoodCount = foodCount;
        ScoreBoard = new ScoreBoard();
        Delay = delay;
        _gameStateUpdater = new GameStateUpdater(Snakes, Field, ScoreBoard, FoodCount);
    }

    public void UpdateGameContext(List<(int x, int y)> food, MyConcurrentList<Snake> snakes, ScoreBoard scores)
    {
        Field = new GameField(Field.GetWidth(), Field.GetHeight());
        foreach (var (x, y) in food)
        {
            Field.SetCell(new FoodCell(x, y));
        }
        foreach (var snake in snakes)
        {
            foreach (var cell in snake.Body)
            {
                Field.SetCell(cell);
            }
        }
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
using System.Text.Json;
using Lab4Core.GameStateControllers;
using Lab4Core.GameObjects;

namespace Lab4Core;

public class GameContext
{
    public GameField Field { get; }
    public List<Snake> Snakes { get; }
    public int FoodCount { get; }
    public ScoreBoard ScoreBoard { get; }
    public int Delay { get; }
    private GameStateUpdater _gameStateUpdater;

    private GameContext(int width, int height, int foodCount, int delay)
    {
        Field = new GameField(width, height);
        Snakes = new List<Snake>();
        FoodCount = foodCount;
        ScoreBoard = new ScoreBoard();
        Delay = delay;
        _gameStateUpdater = new GameStateUpdater(Snakes, Field, ScoreBoard, FoodCount);
    }
    
    public static GameContext InitContext()
    {
        static string Path()
        {
            return "Config/GameContext.json";
        }
        int[]? args = JsonSerializer.Deserialize<int[]>(File.ReadAllText(Path())); 
        return new GameContext(args![0], args[1], args[2], args[3]);
    }

    public void Play()
    {
        _gameStateUpdater.Update();
    }

    public Snake? GetNewSnake() => _gameStateUpdater.SpawnSnake();
}
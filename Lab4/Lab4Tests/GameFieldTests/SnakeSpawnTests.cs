using Lab4Core.GameObjects;
using Lab4Core.GameStateControllers;
using Lab4Core;

namespace Lab4Tests.GameFieldTests;

public class SnakeSpawnTests
{
    [Fact]
    public void SnakeSpawn_SpawnsNewSnakes_Test()
    {
        GameField field = new GameField(7, 7);
        var snakes = new List<Snake>();
        foreach(var i in (int[])[0,3,6]) foreach(var j in (int[])[0,3,6]) 
            if(i != 3 || j != 3) field.SetCell(new SnakeCell(i, j, 1));
        GameStateUpdater updater = new GameStateUpdater(snakes, field, new ScoreBoard());
        
        updater.SpawnSnake(1);

        Assert.IsType<SnakeCell>(field.GetCell(3, 3));
        var tail = new List<(int, int)>{(3,2), (3,4), (2,3), (4,3)};
        Assert.Contains(tail, coords => field.GetCell(coords.Item1,coords.Item2) is SnakeCell);
    }

    [Fact]
    public void SnakeSpawn_DoesntSpawnSnakesWhenNoPlace_Test()
    {
        GameField field = new GameField(2, 2);
        GameStateUpdater updater = new GameStateUpdater([], field, new ScoreBoard());
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                field.SetCell(new FoodCell(i, j));
            }
        }
        
        updater.SpawnSnake(1);

        for (int y = 0; y < field.GetHeight(); y++)
        {
            for (int x = 0; x < field.GetWidth(); x++)
            {
                Assert.IsNotType<SnakeCell>(field.GetCell(x, y));
            }
        }
    }
}
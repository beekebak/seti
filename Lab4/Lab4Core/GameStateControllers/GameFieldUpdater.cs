using Lab4Core.GameObjects;

namespace Lab4Core.GameStateControllers;

class GameFieldUpdater
{
    private void MoveAliveSnakes(List<Snake> snakes, GameField gameField)
    {
        snakes.ForEach(snake => snake.Body.ToList().ForEach(gameField.SetCell));
    }

    private void ClearOldSnakes(GameField gameField)
    {
        for (int y = 0; y < gameField.GetHeight(); y++)
        {
            for (int x = 0; x < gameField.GetWidth(); x++)
            {
                if(gameField.GetCell(x, y) is SnakeCell) gameField.SetCell(new EmptyCell(x, y));
            }
        }
    }

    private void DestroySnakes(GameField gameField, HashSet<Snake> removedSnakes)
    {
        var random = new Random();
        removedSnakes.ToList().ForEach(snake => snake.Body.ToList().ForEach(cell =>
        {
            if (random.Next(2) == 0)
            {
                int x = cell.GetPosition().Item1;
                int y = cell.GetPosition().Item2;
                gameField.SetCell(new FoodCell(x, y));
            }
        }));
    }

    private void FixFood(GameField gameField, int minFoodCount)
    {
        int foodCount = 0;
        List<Cell> empties = new List<Cell>();
        for(var y = 0; y < gameField.GetHeight(); y++)
        {
            for (var x = 0; x < gameField.GetWidth(); x++)
            {
                foodCount += gameField.GetCell(x, y) is FoodCell ? 1 : 0;
                if(gameField.GetCell(x, y) is EmptyCell cell) empties.Add(cell);
            }
        }

        if (foodCount < minFoodCount)
        {
            Random rnd = new Random();
            List<int> indexes = Enumerable.Range(0, empties.Count-1).ToList();
            var randomIndexes = indexes.OrderBy(x => rnd.Next()).Take(minFoodCount-foodCount).ToList();
            foreach (var index in randomIndexes)
            {
                int x = empties[index].GetPosition().Item1;
                int y = empties[index].GetPosition().Item2;
                gameField.SetCell(new FoodCell(x,y));
            }
        }
    }
    
    public void UpdateGameField(List<Snake> snakes, HashSet<Snake> removedSnakes, GameField gameField,
        int minFoodCount)
    {
        ClearOldSnakes(gameField);
        MoveAliveSnakes(snakes, gameField);
        DestroySnakes(gameField, removedSnakes);
        FixFood(gameField, minFoodCount);
    }
}
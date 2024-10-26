using Lab4Core.GameObjects;

namespace Lab4Core.GameStateControllers;

class ScoreBoardUpdater
{
    private void UpdateFoodScore(List<Snake> aliveSnakes, HashSet<Snake> removedSnakes, GameField gameField,
        ScoreBoard scoreBoard)
    {
        UpdateFoodScoreImpl(aliveSnakes, gameField, scoreBoard);
        UpdateFoodScoreImpl(removedSnakes, gameField, scoreBoard);
    }

    private void UpdateFoodScoreImpl(IEnumerable<Snake> collection, GameField gameField, ScoreBoard scoreBoard)
    {
        foreach (var snake in collection)
        {
            int x = snake.GetHead().GetPosition().Item1;
            int y = snake.GetHead().GetPosition().Item2;
            if(gameField.GetCell(x, y) is FoodCell) scoreBoard.UpdateScore(snake, 1);
        }
    }

    private Snake? GetKillerSnake(IEnumerable<Snake> snakes, Snake removed)
    {
        SnakeCell cell = removed.GetHead();
        foreach (var snake in snakes)
        {
            if(snake.ContainsCell(cell) && snake != removed) return snake;
        }
        return null;
    }

    private void UpdateKillScore(List<Snake> aliveSnakes, HashSet<Snake> removedSnakes, ScoreBoard scoreBoard)
    {
        foreach (var removedSnake in removedSnakes)
        {
            Snake? killer = GetKillerSnake(aliveSnakes, removedSnake) ??
                           GetKillerSnake(removedSnakes, removedSnake);
            if(killer != null) scoreBoard.UpdateScore(killer, 1);
        }
    }

    private void SetZeroScoreByDefault(List<Snake> aliveSnakes, HashSet<Snake> removedSnakes, ScoreBoard scoreBoard)
    {
        foreach (var snake in removedSnakes)
        {
            scoreBoard.UpdateScore(snake, 0);
        }
        foreach (var snake in aliveSnakes)
        {
            scoreBoard.UpdateScore(snake, 0);
        }
    }
    
    public void UpdateScore(List<Snake> aliveSnakes, HashSet<Snake> removedSnakes, GameField gameField,
        ScoreBoard scoreBoard)
    {
        SetZeroScoreByDefault(aliveSnakes, removedSnakes, scoreBoard);
        UpdateFoodScore(aliveSnakes, removedSnakes, gameField, scoreBoard);
        UpdateKillScore(aliveSnakes, removedSnakes, scoreBoard);
    }
}
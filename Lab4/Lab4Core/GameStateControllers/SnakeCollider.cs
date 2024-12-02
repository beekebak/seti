using System.Collections.Concurrent;
using Lab4Core.GameObjects;

namespace Lab4Core.GameStateControllers;

class SnakeCollider
{
    private HashSet<Snake> GetSnakesToRemove(GameField tempField, MyConcurrentList<Snake> snakes,
        Dictionary<SnakeCell, Snake> headToSnake)
    {
        HashSet<Snake> snakeToRemove = new HashSet<Snake>();
        foreach (var snake in snakes)
        {
            foreach (var cell in snake.Body)
            {
                int x = cell.GetPosition().Item1;
                int y = cell.GetPosition().Item2;
                if (tempField.GetCell(x, y) is SnakeCell cellSnake)
                {
                    //if two snakes collide at least one of collisions cells should be snake head
                    if(headToSnake.TryGetValue(cellSnake, out var value)) snakeToRemove.Add(value);
                    if(headToSnake.TryGetValue(cell, out var secValue)) snakeToRemove.Add(secValue);
                }
                else
                {
                    tempField.SetCell(cell);
                }
            }
        }
        return snakeToRemove;
    }
    
    public void CollideSnakes(MyConcurrentList<Snake> snakes, out HashSet<Snake> removedSnakes, int width, int height)
    {
        var headToSnake = new Dictionary<SnakeCell, Snake>();
        snakes.ToList().ForEach(snake => headToSnake.Add(snake.GetHead(), snake));
        GameField tempField = new GameField(width, height);
        var snakesToRemove = GetSnakesToRemove(tempField, snakes, headToSnake);
        snakes.RemoveAll(snake => snakesToRemove.Contains(snake));
        removedSnakes = snakesToRemove;
    }
}
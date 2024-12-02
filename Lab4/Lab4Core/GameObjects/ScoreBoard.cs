using System.Collections.Concurrent;

namespace Lab4Core.GameObjects;

public class ScoreBoard
{
    private readonly ConcurrentDictionary<Snake, int> _scores = new ConcurrentDictionary<Snake, int>();

    public void UpdateScore(Snake snake, int scoreDiff)
    {
        if(!_scores.TryAdd(snake, scoreDiff)) _scores[snake] += scoreDiff;
    }
    
    public int GetScore(Snake snake)
    {
        return _scores.GetValueOrDefault(snake, 0);
    }

    public List<KeyValuePair<Snake, int>> GetSnakesAndScores()
    {
        return _scores.ToList();
    }
}
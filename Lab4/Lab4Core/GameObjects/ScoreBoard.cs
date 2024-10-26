namespace Lab4Core.GameObjects;

public class ScoreBoard
{
    private readonly Dictionary<Snake, int> _scores = new Dictionary<Snake, int>();

    public void UpdateScore(Snake snake, int scoreDiff)
    {
        if(!_scores.TryAdd(snake, scoreDiff)) _scores[snake] += scoreDiff;
    }
    
    public int GetScore(Snake snake)
    {
        return _scores[snake];
    }

    public List<KeyValuePair<Snake, int>> GetSnakesAndScores()
    {
        return _scores.ToList();
    }
}
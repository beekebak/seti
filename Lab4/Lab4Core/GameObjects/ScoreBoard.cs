using Microsoft.Win32.SafeHandles;

namespace Lab4Core.GameObjects;

public class ScoreBoard
{
    private Dictionary<Snake, int> _scores = new Dictionary<Snake, int>();

    public void UpdateScore(Snake snake, int scoreDiff)
    {
        if(_scores.ContainsKey(snake)) _scores[snake] += scoreDiff;
        else _scores.Add(snake, scoreDiff);
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
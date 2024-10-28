using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Avalonia.Threading;
using Lab4Core;
using ReactiveUI;

namespace Lab4Frontend.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
#pragma warning disable CA1822 // Mark members as static
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int WidthStable { get; set; }
    public int HeightStable { get; set; }
    
    public ObservableCollection<string> ScoreTable { get; } = new ObservableCollection<string>();
    public ObservableCollection<ObservableCollection<SolidColorBrush>> FieldColors { get; } =
        new ObservableCollection<ObservableCollection<SolidColorBrush>>();
    private static readonly List<Color> _allColors = [Colors.Aqua, Colors.Blue, Colors.Brown, Colors.Crimson,
        Colors.DarkMagenta, Colors.Gray, Colors.Lime, Colors.Magenta, Colors.Khaki, Colors.Orange]; 
    private readonly Dictionary<int, Color> _snakeColorToRealColorMap = new()
    {
        { 0, Colors.Black },
        { 1, Colors.Green }
    };
    private readonly GameContext _context = new GameContext();
    private DispatcherTimer? _timer;
    
    public void StartGame()
    {
        _timer?.Stop();
        _context.InitContext(Height, Width);
        WidthStable = _context.Field.GetWidth();
        HeightStable = _context.Field.GetHeight();
        _context.UpdateGameState(1);
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    public void EndGame()
    {
        _timer?.Stop();
    }

    private void UpdateScoreTableView()
    {
        var scoreTable = _context.ScoreBoard;
        ScoreTable.Clear();
        foreach (var score in scoreTable.GetSnakesAndScores())
        {
            ScoreTable.Add(score.Key.Color.ToString() + "   :   " +score.Value.ToString());
        }
    }

    private Color GetCurrentColor(int x, int y)
    {
        int cellColor = _context.Field.GetCell(x, y).GetColorId();
        if(_snakeColorToRealColorMap.TryGetValue(cellColor, out var color)) return color;
        var random = new Random();
        _snakeColorToRealColorMap.Add(cellColor, _allColors.OrderBy(_ => random.Next())
            .Where(currcolor => !_snakeColorToRealColorMap.ContainsValue(currcolor))
            .FirstOrDefault(Colors.White));
        return _snakeColorToRealColorMap[cellColor];
    }

    private void UpdateGameFieldView()
    {
        FieldColors.Clear();
        for (int y = 0; y < _context.Field.GetHeight(); y++)
        {
            var row = new ObservableCollection<SolidColorBrush>();
            for (int x = 0; x < _context.Field.GetWidth(); x++)
            {
                Color color = GetCurrentColor(x, y);
                row.Add(new SolidColorBrush(color));
            }
            FieldColors.Add(row);
        }
    }
    
    private void OnTimerTick(object? sender, EventArgs e)
    {
        _context.UpdateGameState();
        UpdateScoreTableView();
        UpdateGameFieldView();
    }
#pragma warning restore CA1822 // Mark members as static
}
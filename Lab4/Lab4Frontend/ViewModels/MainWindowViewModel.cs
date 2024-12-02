using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Lab4.NetworkService;
using Lab4.NetworkService.Players;
using Lab4Core;
using ReactiveUI;

namespace Lab4Frontend.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
#pragma warning disable CA1822 // Mark members as static
    public ObservableCollection<string> ScoreTable { get; } = new ObservableCollection<string>();
    public ObservableCollection<ObservableCollection<SolidColorBrush>> FieldColors { get; } =
        new ObservableCollection<ObservableCollection<SolidColorBrush>>();

    public ICommand WKey { get; }
    public ICommand AKey { get; }
    public ICommand SKey { get; }
    public ICommand DKey { get; }

    private static readonly List<Color> AllColors = [Colors.Aqua, Colors.Blue, Colors.Brown, Colors.Crimson,
        Colors.DarkMagenta, Colors.Gray, Colors.LemonChiffon, Colors.Magenta, Colors.Khaki, Colors.Orange]; 
    private readonly Dictionary<int, Color> _snakeColorToRealColorMap = new()
    {
        { 0, Colors.Black },
        { -1, Colors.Green }
    };
    private Game _game;
    
    public MainWindowViewModel()
    {
        WKey = ReactiveCommand.Create(MoveDown);
        AKey = ReactiveCommand.Create(MoveLeft);
        SKey = ReactiveCommand.Create(MoveUp);
        DKey = ReactiveCommand.Create(MoveRight);
        _game = new Game();
        _game.Start();
    }

    private void MoveUp() => _game.Move(Directions.Up);
    private void MoveLeft() => _game.Move(Directions.Left);
    private void MoveDown() => _game.Move(Directions.Down);
    private void MoveRight() => _game.Move(Directions.Right);
    
    public void StartGame()
    {
        _game.InitializeGameStart();
        _game.ModelUpdated += OnModelUpdate;
    }

    public async void EndGame()
    {
        await _game.LeaveGame();
        _snakeColorToRealColorMap.Clear();
    }

    private void UpdateScoreTableView()
    {
        var scoreTable = _game.GetScores();
        ScoreTable.Clear();
        foreach (var score in scoreTable)
        {
            ScoreTable.Add(score.Item1 + "   :   " +score.Item2);
        }
    }

    private Color GetCurrentColor(int x, int y)
    {
        int cellColor = _game.GetCellColor(x, y);
        if(_snakeColorToRealColorMap.TryGetValue(cellColor, out var color)) return color;
        var random = new Random();
        _snakeColorToRealColorMap.Add(cellColor, AllColors.OrderBy(_ => random.Next())
            .Where(currcolor => !_snakeColorToRealColorMap.ContainsValue(currcolor))
            .FirstOrDefault(Colors.White));
        return _snakeColorToRealColorMap[cellColor];
    }

    private void UpdateGameFieldView()
    {
        FieldColors.Clear();
        for (int y = 0; y < _game.GetFieldHeight(); y++)
        {
            var row = new ObservableCollection<SolidColorBrush>();
            for (int x = 0; x < _game.GetFieldWidth(); x++)
            {
                Color color = GetCurrentColor(x, y);
                row.Add(new SolidColorBrush(color));
            }
            FieldColors.Add(row);
        }
    }
    
    private void OnModelUpdate()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            UpdateScoreTableView();
            UpdateGameFieldView();
        });
    }
#pragma warning restore CA1822 // Mark members as static
}
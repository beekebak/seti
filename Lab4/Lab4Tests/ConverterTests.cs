using System.Data;
using System.Net;
using Lab4.NetworkService;
using Lab4.NetworkService.Players;
using Lab4.NetworkService.Wrappers;
using Lab4Core;
using Lab4Core.GameObjects;

namespace Lab4Tests;

public class ConverterTests
{
    [Fact]
    public void ConverterTests_SnakeConversionDoesntChangeSnake_Test()
    {
        List<(int, int)> body = new List<(int, int)>();
        body.AddRange([(1,2), (1,3), (1,4), (2,4), (2,3), (3,3)]);
        Snake snake = new Snake(body, 1, Directions.Down);
        
        var dto = Converter.GetSnakeDto(snake);
        var parsed = Converter.GetSnake(dto);
        
        Assert.True(parsed.Alive);
        Assert.Equal(1, parsed.PlayerId);
        Assert.Equal(Directions.Down, parsed.Direction);
        for (int i = 0; i < body.Count; i++)
        {
            Assert.Equal(body[i].Item2, parsed.Body[i].GetPosition().y);
            Assert.Equal(body[i].Item1, parsed.Body[i].GetPosition().x);
        }
    }

    [Fact]
    public void ConverterTests_StateConversionDoesntChangeState_Test()
    {
        var config = new GameConfig
        {
            Food = 5,
            Height = 20,
            Name = "newgame",
            Timeout = 100,
            Width = 15
        };
        MultiplayerGameContext context = MultiplayerGameContext.LoadGame(config);
        MyConcurrentList<Player> players = [new Player(PlayerRole.Master), new Player(PlayerRole.Deputy)];
        context.Players = players;
        List<(int, int)> firstBody = new List<(int, int)>();
        List<(int, int)> secondBody = new List<(int, int)>();
        firstBody.AddRange([(1,2), (1,3), (1,4)]);
        secondBody.AddRange([(3,2), (4,2), (5,2)]);
        players[0].UpdateData(1, "master", new Snake(firstBody, 1, Directions.Up));
        players[1].UpdateData(2, "deputy", new Snake(secondBody, 2, Directions.Down), true);
        var actualBoard = new ScoreBoard();
        actualBoard.UpdateScore(players[0].RelatedSnake, 10);
        actualBoard.UpdateScore(players[1].RelatedSnake, 0);
        context.UpdateGame(new List<(int x, int y)> { (10, 2), (3, 11), (7, 8) }, players,
            actualBoard);
        List<IPEndPoint> endPoints = [new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080),
            new IPEndPoint(IPAddress.Parse("127.2.10.132"), 11110)];
        var wrappers = context.Players.Select(p => new PlayerWrapper(p)).ToList();

        var dto = Converter.GetGameStateDto(context, wrappers);
        Converter.ParseGameStateDto(dto, out List<(int x, int y)> foods, out MyConcurrentList<PlayerWrapper> playerWrappers, 
            out var board);

        List<(int x, int y)> actualFood = context.Context.Field.GetFoodPositions();
        for (int i = 0; i < actualFood.Count; i++)
        {
            Assert.Equal(actualFood[i].x, foods[i].x);
            Assert.Equal(actualFood[i].y, foods[i].y);
        }
        for (int i = 0; i < players.Count; i++)
        {
            Assert.Equal(playerWrappers[i].Player.Name, players[i].Name);
            Assert.Equal(playerWrappers[i].Player.Id, players[i].Id);
            Assert.Equal(playerWrappers[i].Player.IsAlive, players[i].IsAlive);
            Assert.Equal(playerWrappers[i].Player.Role, players[i].Role);
            Assert.Equal(playerWrappers[i].Player.RelatedSnake!.Direction, players[i].RelatedSnake!.Direction);
            Assert.Equal(playerWrappers[i].Player.RelatedSnake!.PlayerId, players[i].RelatedSnake!.PlayerId);
            Assert.Equal(playerWrappers[i].Player.Id, players[i].RelatedSnake!.PlayerId);
            Assert.Equal(playerWrappers[i].Player.RelatedSnake!.Alive, players[i].RelatedSnake!.Alive);
            Assert.Equal(playerWrappers[i].EndPoint, wrappers[i].EndPoint);
            for (int j = 0; j < playerWrappers[i].Player.RelatedSnake!.Body.Count; j++)
            {
                Assert.Equal(playerWrappers[i].Player.RelatedSnake!.Body[j].GetPosition().x, 
                    players[i].RelatedSnake!.Body[j].GetPosition().x);
                Assert.Equal(playerWrappers[i].Player.RelatedSnake!.Body[j].GetPosition().y, 
                    players[i].RelatedSnake!.Body[j].GetPosition().y);
            }
        }

        for (int i = 0; i < board.GetSnakesAndScores().Count; i++)
        {
            Assert.Equal(10, board.GetScore(players[0].RelatedSnake!));
            Assert.Equal(0, board.GetScore(players[1].RelatedSnake!));
        }
    }
}
using Lab4.NetworkService.NetworkLogicClasses;
using Lab4Core;
using Lab4Core.GameObjects;
using GameConfig = Lab4Core.GameConfig;

namespace Lab4.NetworkService.Players;

public class Player
{
    public Snake? RelatedSnake { get; set; }

    public string Name { get; protected set; } = "<blank>";
    //public int Id { get; protected set; } = NetworkContext.GetNewPlayerIndex();
    public int Id { get; set; }
    public bool IsAlive { get; protected set; }
    public PlayerRole Role { get; set; }

    public Player(PlayerRole role)
    {
        Role = role;
    }
    
    public bool Move(Directions direction)
    {
        if(RelatedSnake == null) return false;
        if(Role == PlayerRole.Viewer) return false;
        if(direction == RelatedSnake.GetForbiddenDirection()) return false;
        RelatedSnake.Direction = direction;
        return true;
    }

    public void UpdateData(int id, string name, Snake? relatedSnake, bool alive = true)
    {
        Id = id;
        Name = name;
        RelatedSnake = relatedSnake;
        IsAlive = alive;
        relatedSnake!.Alive = IsAlive;
    }

    public void UpdateData(Player player)
    {
        Id = player.Id;
        Name = player.Name;
        RelatedSnake = player.RelatedSnake;
        IsAlive = player.IsAlive;
    }

    public void SetupData(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
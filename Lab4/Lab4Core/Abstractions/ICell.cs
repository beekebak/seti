namespace Lab4Core.Abstractions;

public interface ICell 
{ 
    public int GetColorId();
    public (int, int) GetPosition();
}
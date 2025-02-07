namespace TextRPG.Objects;

public class ObjectContext
{
    public Player Player = new Player("Player");
    public Dungeon Dungeon = new Dungeon();
    public Battle Battle = new Battle();
    public Shop.Shop Shop = new Shop.Shop();
    public GameManager GameManager = new GameManager();
}
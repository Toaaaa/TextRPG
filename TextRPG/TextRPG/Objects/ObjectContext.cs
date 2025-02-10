namespace TextRPG.Objects;

public class ObjectContext
{
    public static ObjectContext Instance { get; private set; } = new ObjectContext();

    public Player Player = new Player("Player");
    public Dungeon Dungeon = new Dungeon();
    public Battle Battle = new Battle();
    public Shop.Shop Shop = new Shop.Shop();
}
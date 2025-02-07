namespace TextRPG.Objects;

public class Player
{
    public int playerLevel { get; set; }
    public string playerName { get; set; }
    public int Attack { get; set; }
    public int AddAttack { get; set; }
    public int Defense { get; set; }
    public int AddDefense { get; set; }
    public int HP { get; set; }
    public int AddHP { get; set; }
    public int Gold { get; set; }
    public int Exp { get; set; }
    public int Speed { get; set; }
    public List<Item> Inventory { get; set; }

    public Player(string name)
    {
        playerLevel = 1;
        playerName = name;
        Attack = 10;
        AddAttack = 0;
        Defense = 5;
        AddDefense = 0;
        HP = 50;
        AddHP = 0;
        Gold = 1500;
        Exp = 0;
        Speed = 5;
        Inventory = new List<Item>();
    }
    
    // 아이템 장착
    public void EquipItem(Item item)
    {
        // 기존에 장착된 아이템중 동일한 타입의 아이템이 있다면 해제하고 장착
        foreach (var i in Inventory)
        {
            if (i.Type == item.Type && i.IsEquipped)
            {
                UnequipItem(i);
            }
        }
        item.IsEquipped = true;
    }

    // 아이템 해제
    public void UnequipItem(Item item)
    {
        item.IsEquipped = false;
    }
    
    // 아이템 획득
    public void GetItem(Item item)
    {
        Inventory.Add(item);
    }

    // 아이템 삭제
    public void RemoveItem(Item item)
    {
        Inventory.Remove(item);
    }
    
    // 데미지 계산
    public float CalcDamage()
    {
        var random = new Random();
        var damage = (Attack + AddAttack) * (random.Next(90, 111) / 100);
        return damage;
    }
    
    // 데미지 받아서 체력 -
    public float TakeDamage(int damage)
    {
        HP = (HP <= damage) ? 0 : HP - damage;
        return HP;
    }
    
    public bool LevelUp()
    {
        playerLevel++;
        Attack += 5;
        Defense += 2;
        Exp = 0;
        return true;
    }
    
    public bool GetExp(int exp)
    {
        bool isLevelUp = false;
        Exp += exp;
        if (Exp >= playerLevel * 100)
        {
            isLevelUp = LevelUp();
        }
        return isLevelUp;
    }
}
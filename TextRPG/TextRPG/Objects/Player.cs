using TextRPG.Objects.Items;

namespace TextRPG.Objects;

public class Player : Actor
{
    public int AddAttack { get; set; }
    public int AddDefense { get; set; }
    public int AddHP { get; set; }
    public List<Item> Inventory { get; set; }

    public Player(string name)
    {
        Level = 1;
        Name = name;
        ATK = 10;
        AddAttack = 0;
        DEF = 5;
        AddDefense = 0;
        MaxHP = 50;
        HP = 50;
        AddHP = 0;
        Gold = 1500;
        EXP = 0;
        SPD = 5;
        Inventory = new List<Item>();
    }
    
    // 아이템 장착
    public void EquipItem(EquipItem item)
    {
        foreach (var i in Inventory.OfType<EquipItem>()) // EquipItem만 필터링
        {
            if (i.Part == item.Part && i.IsEquip)
            {
                UnequipItem(i);
            }
        }
        item.IsEquip = true;
    }

    // 아이템 해제
    public void UnequipItem(EquipItem item)
    {
        item.IsEquip = false;
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
        var damage = (ATK + AddAttack) * (random.Next(90, 111) / 100);
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
        Level++;
        MaxHP += Level * 5;
        HP = MaxHP;
        ATK += 5;
        DEF += 2;
        EXP = 0;
        return true;
    }
    
    public bool GetExp(int exp)
    {
        bool isLevelUp = false;
        EXP += exp;
        if (EXP >= Level * 100)
        {
            isLevelUp = LevelUp();
        }
        return isLevelUp;
    }
}
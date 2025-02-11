using TextRPG.Objects.Items;

namespace TextRPG.Objects;

public class Player : Actor
{
    public int AddAttack { get; set; }
    public int AddDefense { get; set; }
    public int AddHP { get; set; }
    public string Class { get; set; }
    public List<Item> Inventory { get; set; }
    public int MaxMP { get; set; }
    public int MP { get; set; }

    public Player(string name)
    {
        Level = 1;
        Name = name;
        Class = "Warrior";
        TotalATK = 10;
        AddAttack = 0;
        TotalDEF = 5;
        AddDefense = 0;
        MaxHP = 50;
        HP = 50;
        MaxMP = 30;
        MP = 30;
        AddHP = 0;
        Gold = 1500;
        EXP = 0;
        SPD = 5;
        CRIT = 10;
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
        UpdateStat(true, item.Part == EEquipPart.Weapon, item.TotalStat);
        item.IsEquip = true;
    }

    // 아이템 해제
    public void UnequipItem(EquipItem item)
    {
        UpdateStat(false, item.Part == EEquipPart.Weapon, item.TotalStat);
        item.IsEquip = false;
    }
    
    /*
     * 아이템 장착 시 스탯 추가, 해제 시 스탯 감소
     * @param isEquipped 장착 여부
     * @param isAttackStat 공격력인지 방어력인지
     * @param statChange 변경할 스탯
     */
    public void UpdateStat(bool isEquipped, bool isAttackStat, int statChange)
    {
        if (isEquipped)
        {
            // 아이템 장착 시 스탯 추가
            if (isAttackStat)
            {
                AddAttack += statChange;
                TotalATK += statChange;
            }
            else
            {
                AddDefense += statChange;
                TotalDEF += statChange;
            }
        }
        else
        {
            // 아이템 해제 시 스탯 감소
            if (isAttackStat)
            {
                AddAttack -= statChange;
                TotalATK -= statChange;
            }
            else
            {
                AddDefense -= statChange;
                TotalDEF -= statChange;
            }
        }
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
    public int CalcDamage()
    {
        var random = new Random();
        // 100.0으로 나누어 소수점을 반영
        int damage = (int)(TotalATK * (random.Next(90, 111) / 100.0f));
        return damage;
    }
    
    // 스킬 데미지 계산
    public int SkillDamage(Skill skill)
    {
        int damage = CalcDamage();
        int skillDamage = damage + (int)(damage * (skill.Percent / 100.0f));
        return skillDamage;
    }
    
    // 데미지 받아서 체력 -
    public int TakeDamage(int damage)
    {
        HP = (HP <= damage) ? 0 : HP - damage;
        return HP;
    }
    
    public bool LevelUp()
    {
        Level++;
        MaxHP += Level * 5;
        MaxMP += Level * 3;
        HP = MaxHP;
        MP = MaxMP;
        TotalATK += 5;
        TotalDEF += 2;
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
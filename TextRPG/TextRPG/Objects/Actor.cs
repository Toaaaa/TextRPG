namespace TextRPG.Objects;

public class Actor
{
    public int Level { get; set; }
    public string Name { get; set; }
    public int TotalATK { get; set; }
    public int TotalDEF { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int Gold { get; set; }
    public int EXP { get; set; }
    public int SPD { get; set; }
    
    // 데미지 계산
    public virtual int CalcDamage()
    {
        var random = new Random();
        var damage = TotalATK * (random.Next(90, 111) / 100);
        return damage;
    }
    
    // 데미지 받아서 체력 -
    public virtual int TakeDamage(int damage)
    {
        HP = (HP <= damage) ? 0 : HP - damage;
        return HP;
    }
}
namespace TextRPG.Objects;

public class Actor
{
    public int Level { get; set; }
    public string Name { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int Gold { get; set; }
    public int EXP { get; set; }
    public int SPD { get; set; }
    
    // 데미지 계산
    public virtual float CalcDamage()
    {
        var random = new Random();
        var damage = ATK * (random.Next(90, 111) / 100);
        return damage;
    }
    
    // 데미지 받아서 체력 -
    public virtual float TakeDamage(int damage)
    {
        HP = (HP <= damage) ? 0 : HP - damage;
        return HP;
    }
}
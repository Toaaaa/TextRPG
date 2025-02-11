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
    public int CRIT { get; set; }
    
    // 데미지 계산
    public virtual int CalcDamage()
    {
        var random = new Random();
        int damage = (int)(TotalATK * (random.Next(90, 111) / 100.0f));
        return damage;
    }
    
    // 데미지 받아서 체력 -
    public virtual int TakeDamage(int damage)
    {
        HP = (HP <= damage) ? 0 : HP - damage;
        return HP;
    }

    public virtual bool IsCritical()
    {
        var random = new Random();
        return random.Next(0, 101) <= CRIT;
    }

    // 데이터 복사 메서드
    public virtual Actor Clone()
    {
        return (Actor)this.MemberwiseClone();
    }
}
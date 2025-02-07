using System;

// 몬스터 기본 클래스
public class Monster
{
    public string Name { get; }
    public int Level { get; }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SPD { get; set; }
    public int EXP { get; set; }
   
    // 몬스터 생성
    public Monster(string name, int level, int hp, int atk, int def, int spd, int exp)
    {
        Name = name;
        Level = level;
        HP = hp;
        ATK = atk;
        DEF = def;
        SPD = spd;
        EXP = exp;
    }

    static void Main()
    {
       Monster snail = new Monster("달팽이", 1, 10, 2, 0, 1, 1);
       Monster goblin = new Monster("고블린", 3, 25, 5, 0, 1, 2);
       Monster orc = new Monster("오크", 5, 50, 2, 2, 3, 5);
       Monster golem = new Monster("골렘", 10, 200, 10, 5, 2, 8);

       Monster[] monsters = { snail, goblin, orc, golem };

       foreach (var monster in monsters)
       {
            Console.WriteLine($"{monster.Name} (Lv.{monster.Level})  HP: {monster.HP}, ATK: {monster.ATK}, DEF: {monster.DEF}, SPD: {monster.SPD}, EXP: {monster.EXP}\n");
       }
    }
}


using System;
using TextRPG.Objects;

// 몬스터 기본 클래스
public class Monster : Actor
{
    public string Name { get; }
    public int Level { get; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SPD { get; set; }
    public int EXP { get; set; }
    
    // ✅ 던전별 등장 확률 (난이도 1~4)
    public Dictionary<int, int> SpawnRates { get; set; }


    // 사망 여부

    public bool IsDead
    {
    get { return HP <= 0; }
    }
   
    // 몬스터 생성
    public Monster(string name, int level, int maxhp, int atk, int def, int spd, int exp)
    {
        Name = name;
        Level = level;
        MaxHP = maxhp;
        HP = maxhp;
        ATK = atk;
        DEF = def;
        SPD = spd;
        EXP = exp;
        SpawnRates = spawnRates;
    }

    
    // ✅ 몬스터 프리셋 (등장 확률 포함)
    public static List<Monster> MonsterPresets = new List<Monster>
    {
        new Monster("달팽이", 1, 10, 2, 0, 1, 1, new Dictionary<int, int> { {1, 50}, {2, 40}, {3, 0},  {4, 0}  }),
        new Monster("고블린", 3, 25, 5, 0, 1, 2, new Dictionary<int, int> { {1, 50}, {2, 40}, {3, 50}, {4, 0}  }),
        new Monster("오크", 5, 50, 2, 2, 3, 5, new Dictionary<int, int> { {1, 0},  {2, 20}, {3, 50}, {4, 60} }),
        new Monster("골렘", 10, 200, 10, 5, 2, 8, new Dictionary<int, int> { {1, 0},  {2, 0},  {3, 0},  {4, 40} })
    };
}


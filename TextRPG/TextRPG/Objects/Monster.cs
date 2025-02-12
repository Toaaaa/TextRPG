using System;
using TextRPG.Objects;
using System.Collections.Generic;

// 몬스터 기본 클래스
public class Monster : Actor
{
    public int DEF { get; set; }

    // ✅ 드랍 테이블 (아이템, 드랍 확률 %)
    public Dictionary<string, int> DropTable { get; set; }
    

    
    
    // ✅ 던전별 등장 확률 (난이도 1~4)
    public Dictionary<int, int> SpawnRates { get; set; }

    public List<string> DroppedItems { get; private set; } // ✅ 몬스터가 생성될 때 드랍 아이템 결정

    // ✅ 몬스터의 이전 HP 
    public int BeforeHP { get; set; }

    //처치 시 골드
    public int GoldReward{ get; set; }


    // 사망 여부

    public bool IsDead
    {
    get { return HP <= 0; }
    }
   
    // 몬스터 생성
    public Monster(string name, int level, int maxhp, int atk, int def, int spd, int exp, int goldReward, Dictionary<int, int> spawnRates, Dictionary<string, int> dropTable, int crit = 5)
    {
        Name = name;
        Level = level;
        MaxHP = maxhp;
        HP = maxhp;
        BeforeHP = maxhp;
        TotalATK = atk;
        DEF = def;
        SPD = spd;
        EXP = exp;
        GoldReward = goldReward;
        SpawnRates = spawnRates;
        DropTable = dropTable;
        CRIT = crit;

        // ✅ 몬스터 생성 시 어떤 아이템을 드랍할지 결정
        DroppedItems = DetermineDroppedItems();
    }

    // ✅ 몬스터 생성 시 드랍 아이템 결정 메서드
    private List<string> DetermineDroppedItems()
    {
        List<string> droppedItems = new List<string>();
        Random rand = new Random();

        foreach (var item in DropTable)
        {
            int roll = rand.Next(1, 101); // 1~100 사이 랜덤 값
            if (roll <= item.Value) // 확률 체크
            {
                droppedItems.Add(item.Key);
            }
        }

        return droppedItems;
    }

    
    
    // ✅ 몬스터 프리셋 (등장 확률 포함)
    public static List<Monster> MonsterPresets = new List<Monster>
    {
        new Monster("달팽이", 1, 10, 2, 0, 1, 1, 100, new Dictionary<int, int> { {1, 50}, {2, 40}, {3, 0},  {4, 0}  },new Dictionary<string, int> { {"포션", 30}, {"강화석", 30},}, 5), 
        new Monster("고블린", 3, 25, 5, 0, 1, 2, 200, new Dictionary<int, int> { {1, 50}, {2, 40}, {3, 50}, {4, 0}  }, new Dictionary<string, int> { {"포션", 40}, {"강화석", 35},}, 5) ,
        new Monster("오크", 5, 50, 2, 2, 3, 5, 300, new Dictionary<int, int> { {1, 0},  {2, 20}, {3, 50}, {4, 60} },  new Dictionary<string, int> { {"포션", 50}, {"하이포션", 20}, {"강화석", 40},}, 5 ),
        new Monster("골렘", 10, 200, 10, 5, 2, 8, 500, new Dictionary<int, int> { {1, 0},  {2, 0},  {3, 0},  {4, 40} }, new Dictionary<string, int> { {"하이포션", 35}, {"강화석", 50},},5 ),
    };

    

    // ✅ 난이도별 몬스터 생성 메서드
    public static List<Monster> GenerateDungeonMonsters(int difficulty)
    {
        Random rand = new Random();
        List<Monster> dungeonMonsters = new List<Monster>();

        // ✅ 난이도별 몬스터 필터링 (등장 확률이 0보다 큰 몬스터만 포함)
        Dictionary<Monster, int> validMonsters = new Dictionary<Monster, int>();

        foreach (var monster in MonsterPresets)
        {
            if (monster.SpawnRates.ContainsKey(difficulty) && monster.SpawnRates[difficulty] > 0)
            {
                validMonsters.Add(monster, monster.SpawnRates[difficulty]);
            }
        }

        // ✅ 몬스터 개수 설정
        int minCount = 1, maxCount = 1;
        switch (difficulty)
        {
            case 1: minCount = 1; maxCount = 2; break;
            case 2: minCount = 2; maxCount = 3; break;
            case 3: minCount = 2; maxCount = 4; break;
            case 4: minCount = 3; maxCount = 4; break;
            default: throw new ArgumentException("올바른 난이도를 입력하세요! (1~4)");
        }

        int monsterCount = rand.Next(minCount, maxCount + 1);

        // ✅ 확률 기반 몬스터 등장
        for (int i = 0; i < monsterCount; i++)
        {
            int roll = rand.Next(1, 101); // 1~100 사이 랜덤 값
            int cumulativeChance = 0;

            foreach (var entry in validMonsters)
            {
                cumulativeChance += entry.Value;
                if (roll <= cumulativeChance)
                {
                    dungeonMonsters.Add(new Monster(entry.Key.Name, entry.Key.Level, entry.Key.HP, 
                                                    entry.Key.TotalATK, entry.Key.DEF, entry.Key.SPD, entry.Key.EXP, entry.Key.GoldReward,
                                                    entry.Key.SpawnRates, entry.Key.DropTable, entry.Key.CRIT)); // 같은 몬스터라도 새 객체 생성
                    break;
                }
            }
        }
        return dungeonMonsters;
    }
}


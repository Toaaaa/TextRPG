namespace TextRPG.Objects;

public class Dungeon
{
    static List<Monster> _monsters = new List<Monster> //추후 각 몬스터들이 담긴 프리셋은 Monster.cs에서 던전별 프리셋으로 구현 요망.
    {
        new Monster("달팽이", 1, 10, 2, 0, 1, 1),
        new Monster("고블린", 3, 25, 5, 0, 1, 2),
        new Monster("오크", 5, 50, 2, 2, 3, 5),
        new Monster("골렘", 10, 200, 10, 5, 2, 8),
    };

    public List<Stage> stages = new List<Stage>
    {
        new Stage("초급 던전",_monsters),
        new Stage("중급 던전",_monsters),
        new Stage("고급 던전",_monsters),
        new Stage("심연 던전",_monsters),
    };

    private Stage? CurrentStage { get; set; }
    public List<Monster>? MonsterList { get; set; }


    //리턴
    public Stage? GetCurrentStage()
    {
        return CurrentStage;
    }
    public List<Monster>? GetMonsterList()
    {
        return MonsterList;
    }

    //던전
    public void PrintStages()//던전 선택지 출력
    {
        int index = 1;
        foreach (var stage in stages)
        {
            Console.WriteLine($"{index}. {stage.Name}");
            index++;
        }
    }
    public void EnterStage(int stageIndex)//던전 선택
    {
        if (stageIndex < 1 || stageIndex > stages.Count)
        {
            Console.WriteLine("잘못된 선택입니다.");
            return;
        }

        Console.WriteLine($"{stages[stageIndex - 1].Name}에 입장했습니다.");
        CurrentStage = stages[stageIndex - 1];
        MonsterList = Monster.GenerateDungeonMonsters(stageIndex-1);
    }
}

public class Stage
{
    public string Name { get; set; }
    List<Monster> monsters = new List<Monster>();

    public Stage(string name, List<Monster> monsters)
    {
        Name = name;
        this.monsters = monsters;
    }
    public List<Monster> MonsterSet()//몬스터 셋 메서드
    {
        return monsters;
    }
}

namespace TextRPG;

public class Dungeon
{
    List<Stage> stages = new List<Stage>
    {
        new Stage("초급 던전"),
        new Stage("중급 던전"),
        new Stage("고급 던전"),
        new Stage("심연 던전"),
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
        MonsterList = CurrentStage.MonsterSet();
    }
}

public class Stage
{
    public string Name { get; set; }
    public Stage(string name/*, List<Monster> monsters*/)
    {
        Name = name;
        //this.monsters = monsters;
    }
    List<Monster> monsters = new List<Monster>();
    public List<Monster> MonsterSet()//몬스터 셋 메서드
    {
        return monsters;
    }
}

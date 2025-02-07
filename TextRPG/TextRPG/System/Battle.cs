using TextRPG.Objects;

namespace TextRPG;

public class Battle
{
    private int TotalExp { get; set; }
    private int TotalGold { get; set; }
    //public List<Item>? RewardItems { get; set; }


    PriorityQueue<Actor,int> TurnQueue = new PriorityQueue<Actor, int>();
    public List<Monster>? MonsterList { get; set; }

    //데이터 리턴
    public int GetTotalExp()//전투 종료 후 획득할 경험치 반환.
    {
        return TotalExp;
    }
    public int GetTotalGold()//전투 종료 후 획득할 골드 반환.
    {
        return TotalGold;
    }
    /*
    public List<Item> GetItems(List<Item> items)//전투 종료 후 획득할 아이템 반환.
    {
        return items;
    }
    */
    public string GetNowTurnName(string turn)//턴을 진행할 객체 이름 반환.
    {
        return turn;
    }
    public bool GetIsPlayerTurn(bool turn)//플레이어의 턴인지 아닌지 반환. (true: 플레이어 턴, false: 몬스터 턴)
    {
        return turn;
    }   
    //행동 + 선택
    public void PlayerAttack(Player player,Monster monster)//플레이어가 공격 선택시 호출
    {

    }
    public void MonsterTurn()//몬스터의 턴 선택시 호출
    {
        MonsterAttack();
    }
    void MonsterAttack()//몬스터가 공격 선택시 호출
    {

    }
    void OnMonsterDeath(Monster monster)//몬스터가 죽었을 때 호출
    {
        TotalExp += monster.EXP;
        TotalGold += monster.Gold;
    }
    //전투
    private Queue<Actor> GetTurnQueue(List<Actor> actors)//턴 순서 정하기
    {
        foreach (var actor in actors)
        {
            TurnQueue.Enqueue(actor, -actor.Speed);
        }
        return TurnQueue;
    }
    private void BeforeBattle()
    {
        TotalExp = 0;
        TotalGold = 0;
        //몬스터 리스트 가져오기
        TurnQueue = GetTurnQueue(actors);
    }
    private void StartBattle()
    {

    }

    // 1. 전투 시작전 모든 객체들의 속도값을 가져와 턴 순서를 정한다 (사망시 제외)
    // 2. 턴 순서가 정해지면 순서에 맞춰 객체들이 행동을 선택한다. (@@의 턴)
    // 3. 행동을 선택한 객체들이 행동을 수행한다.
}
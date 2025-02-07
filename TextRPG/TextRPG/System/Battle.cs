using TextRPG.Objects;
using TextRPG.Objects.Items;

namespace TextRPG;

public class Battle
{
    private int TotalExp { get; set; }
    private int TotalGold { get; set; }
    public List<Item>? RewardItems { get; set; }
    public Monster? TargetMonster { get; set; }

    public PriorityQueue<Actor,int> TurnQueue = new PriorityQueue<Actor, int>();
    public List<Monster>? MonsterList = new List<Monster>();
    private List<Actor> actors = new List<Actor>();
    

    public Action<Player, Monster>? PlayerAction { get; set; }

    //데이터 리턴
    public int GetTotalExp()//전투 종료 후 획득할 경험치 반환.
    {
        return TotalExp;
    }
    public int GetTotalGold()//전투 종료 후 획득할 골드 반환.
    {
        return TotalGold;
    }
    public List<Item> GetItems(List<Item> items) //전투 종료 후 획득할 아이템 반환.
    {
        return items;
    }
    public List<Monster>? GetMonsterList()//현재 몬스터 리스트 반환.
    {
        return MonsterList;
    }
    public int? GetMonsterCount()//현재 몬스터 수 반환.
    {
        return MonsterList?.Count;
    }
    public string GetMonsterInfoQueue(int quenum)//몬스터 정보 반환.
    {
        if(quenum <= MonsterList?.Count-1)
        {
            Monster? mon = MonsterList?[quenum];
            return $"Lv.{mon?.Level} {mon?.Name}  Hp {mon?.HP}/{mon?.MaxHP}";
        }
        else
        {
            return "몬스터가 없습니다.";
        }
    }
    public Monster? GetMonster(int num)//리스트 에서의 몬스터 반환.
    {
        if(num <= MonsterList?.Count-1)
            return MonsterList?[num];
        else
            return null;
    }
    public bool? GetMonsterIsDead(int num)//몬스터가 죽었는지 반환.
    {
        if(num <= MonsterList?.Count-1)
            return MonsterList?[num].IsDead;
        else
            return false;
    }
    public bool GetIsPlayerTurn(bool turn)//플레이어의 턴인지 아닌지 반환. (true: 플레이어 턴, false: 몬스터 턴)
    {
        return turn;
    }   
    //행동 + 선택
    public void PlayerAttack(Monster monster)//플레이어가 공격 선택시 호출
    {
        int realdmg = ObjectContext.Instance.Player.CalcDamage() * (1 - (monster.DEF / 25 + monster.DEF)); //방어상수 25.
        monster.TakeDamage(realdmg);
    }
    public void MonsterTurn(Player player, Monster monster)//몬스터의 턴 선택시 호출
    {
        MonsterAttack(player,monster);
    }
    void MonsterAttack(Player player,Monster monster)//몬스터가 공격 선택시 호출
    {
        int realdmg = monster.CalcDamage() * (1 - (player.TotalDEF / 25 + player.TotalDEF)); //방어상수 25.
        player.TakeDamage(realdmg);
    }
    void OnMonsterDeath(Monster monster)//몬스터가 죽었을 때 호출
    {
        TotalExp += monster.EXP;
        TotalGold += monster.Gold;
        //아이템 드랍.
    }
    public void SetTargetMonster(Monster monster)//타겟 몬스터 설정
    {
        TargetMonster = monster;
    }
    //전투
    private PriorityQueue<Actor,int> GetTurnQueue(List<Actor> actors)//턴 순서 정하기
    {
        foreach (var actor in actors)
        {
            if(actor is Player)
                TurnQueue.Enqueue(actor, -actor.SPD);
            else if (actor is Monster monster && !monster.IsDead)
            {
                TurnQueue.Enqueue(monster, -monster.SPD);//몬스터가 살아있을 때 만 큐에 추가.
            }

        }
        return TurnQueue;
    }
    private void BeforeBattle()//던전 입장시 (Dungeon.EnterStage 이후)
    {
        TotalExp = 0;
        TotalGold = 0;
        //몬스터 리스트 가져오기
        MonsterList = ObjectContext.Instance.Dungeon.GetMonsterList();//오브젝트 컨텍스트에서 현재 던전 몬스터 가져오기
        MonsterList?.Sort((x, y) => y.SPD.CompareTo(x.SPD));//몬스터 속도가 빠른 순으로 정렬
        actors.Add(ObjectContext.Instance.Player);//플레이어 추가
        if(MonsterList != null)
            actors.AddRange(MonsterList);//몬스터 추가
        TurnQueue = GetTurnQueue(actors);//턴 순서 정하기
    }
    public void TurnStart()//턴 시작시 호출
    {
        if (TurnQueue.Count == 0)
        {
            TurnQueue = GetTurnQueue(actors);
        }
        Actor actor = TurnQueue.Dequeue();
        if (actor is Player && TargetMonster != null)
        {
            //PlayerAction?.Invoke((Player)actor; //타겟 몬스터);
            PlayerAttack(TargetMonster);
        }
        else
        {
            MonsterTurn(ObjectContext.Instance.Player, (Monster)actor);
        }
    }
    public bool TurnEnd()//턴 종료시 호출 (true: 전투 종료, false: 계속 진행)
    {
        //플레이어 사망체크
        for(int i = 0; i < MonsterList?.Count; i++)
        {
            if (!MonsterList[i].IsDead && MonsterList[i].HP <=0)//몬스터 사망 체크.
            {
                OnMonsterDeath(MonsterList[i]);
            }
        }
        return CheckBattleEnd();
    }
    public bool CheckBattleEnd()//모든 몬스터가 죽었는지 확인.
    {
        if(MonsterList == null)
            return true;
        else
            return MonsterList.All(monster => monster.IsDead);
    }

    // 1. 전투 시작전 모든 객체들의 속도값을 가져와 턴 순서를 정한다 (사망시 제외)
    // 2. 턴 순서가 정해지면 순서에 맞춰 객체들이 행동을 선택한다. (@@의 턴)
    // 3. 행동을 선택한 객체들이 행동을 수행한다.
}
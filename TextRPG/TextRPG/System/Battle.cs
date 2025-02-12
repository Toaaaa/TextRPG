using System.Numerics;
using TextRPG.Objects;
using TextRPG.Objects.Items;
using static System.Net.Mime.MediaTypeNames;

namespace TextRPG;

public class Battle
{
    private int TotalExp { get; set; }
    private int TotalGold { get; set; }
    public List<string>? RewardItems { get; set; }//보상 아이템의 스트링 데이터.
    public List<Actor>? Target { get; set; }
    public Actor? CurrentActor { get; set; }

    public PriorityQueue<Actor,int> TurnQueue = new PriorityQueue<Actor, int>();
    public List<Monster>? MonsterList = new List<Monster>();
    private List<Actor> actors = new List<Actor>();

    public static Action? PlayerAction { get; set; }
    //마지막 행동 기록 (몬스터가 플레이어를 공격 할 때만 사용)
    public int LastHp { get; set; }//마지막 전 HP를 저장.
    public int LastMp { get; set; }//마지막 전 MP를 저장.
    public int LastDamage { get; set; }//마지막 행동이 가한 데미지를 저장.
    public bool LastIsCritical { get; set; }//마지막 행동이 크리티컬 했는지 저장.

    //데이터 리턴
    public List<Actor> GetActors()//현재 전투에 참여하는 모든 객체 반환.
    {
        return actors;
    }
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
    public List<Monster>? GetAliveMonsterList()//살아있는 몬스터 리스트 반환.
    {
        return MonsterList?.FindAll(monster => !monster.IsDead);
    }
    public int? GetMonsterCount()//현재 몬스터 수 반환.
    {
        return MonsterList?.Count;
    }
    public string GetMonsterInfoQueue(int quenum)//존재하는 몬스터 정보 반환.
    {
        if(quenum <= MonsterList?.Count-1)
        {
            Monster? mon = MonsterList?[quenum];
            string result = $"Lv.{mon?.Level} {mon?.Name}  ";
            if(mon?.IsDead == true)
                result += "Dead";
            else
                result += $"Hp {mon?.HP}/{mon?.MaxHP}";
            return result;
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
    public bool GetIsPlayerTurn()//플레이어의 턴인지 아닌지 반환. (true: 플레이어 턴, false: 몬스터 턴)
    {
        return TurnQueue.Peek().GetType() == typeof(Player);
    }   
    //행동 + 선택
    public void PlayerAttack(Monster monster)//플레이어가 공격 선택시 호출
    {
        monster.BeforeHP = monster.HP;
        LastIsCritical = ObjectContext.Instance.Player.IsCritical();
        int realdmg = (int)Math.Ceiling((ObjectContext.Instance.Player.CalcDamage() * (1 - (monster.DEF / (20.0 + monster.DEF))))); //방어상수 20.
        realdmg = LastIsCritical ? (int)Math.Ceiling(realdmg * 1.5) : realdmg;
        monster.HP = monster.TakeDamage(realdmg);
        LastDamage = realdmg;
    }
    public void PlayerSkillAttack(Monster monster, Skill skill)//플레이어가 스킬 공격 선택시 호출
    {
        monster.BeforeHP = monster.HP;
        int realdmg;
        LastIsCritical = ObjectContext.Instance.Player.IsCritical();
        if(skill.IgnoreDefense)
            realdmg = ObjectContext.Instance.Player.SkillDamage(skill); //방어무시 스킬
        else
            realdmg = (int)Math.Ceiling((ObjectContext.Instance.Player.SkillDamage(skill) * (1 - (monster.DEF / (20.0 + monster.DEF))))); //방어상수 20.
        realdmg = LastIsCritical ? (int)Math.Ceiling(realdmg * 1.5) : realdmg;
        monster.HP = monster.TakeDamage(realdmg);
        LastDamage = realdmg;
    }
    public void MonsterTurn(Player player, Monster monster)//몬스터의 턴 선택시 호출
    {
        MonsterAttack(player,monster);
        //추후 스킬 등의 기능..
    }
    void MonsterAttack(Player player,Monster monster)//몬스터가 공격 선택시 호출
    {
        LastHp = player.HP;
        LastIsCritical = monster.IsCritical();
        int realdmg = (int)Math.Ceiling((monster.CalcDamage() * (1 - (player.TotalDEF / (20.0 + player.TotalDEF))))); //방어상수 20.
        realdmg = LastIsCritical ? (int)Math.Ceiling(realdmg * 1.5) : realdmg;
        player.HP = player.TakeDamage(realdmg);
        LastDamage = realdmg;
    }
    void OnMonsterDeath(Monster monster)//몬스터가 죽었을 때 호출
    {
        TotalExp += monster.EXP;
        TotalGold += monster.GoldReward;
        RewardItems?.AddRange(monster.DroppedItems);
    }
    public void SetTargetMonster(List<Monster> monsters)//타겟 몬스터 설정
    {
        Target = monsters.ConvertAll<Actor>(x => x);
    }
    //전투
    public PriorityQueue<Actor,int> GetTurnQueue(List<Actor> actors)//턴 순서 정하기
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
    public void BeforeBattle()//던전 입장시 가장 먼저 호출.
    {
        TotalExp = 0;
        TotalGold = 0;
        RewardItems = new List<string>();

        actors.Clear();
        TurnQueue.Clear();
        
        MonsterList = ObjectContext.Instance.Dungeon.GetMonsterList();//현재 던전 몬스터 가져오기
        //만약 레벨스케일링 등 몬스터 데이터 변환을 할 예정이면 여기서 처리.
        MonsterList?.Sort((x, y) => y.SPD.CompareTo(x.SPD));//몬스터 속도가 빠른 순으로 정렬
        actors.Add(ObjectContext.Instance.Player);//플레이어 추가
        if(MonsterList != null)
            actors.AddRange(MonsterList);//몬스터 추가
        TurnQueue = GetTurnQueue(actors);//턴 순서 정하기
    }
    public void TurnStart()//턴 시작시 호출
    {
        Actor actor = TurnQueue.Dequeue();
        CurrentActor = actor;
        if (actor is Player && Target?.Count >0)//플레이어 턴일 때
        {
            LastHp = ObjectContext.Instance.Player.HP;//플레이어 HP 저장
            LastMp = ObjectContext.Instance.Player.MP;//플레이어 MP 저장
            PlayerAction?.Invoke();
            PlayerAction = null;
        }
        else if(actor is Monster monster)//몬스터 턴일 때
        {
            if(monster.IsDead)//죽은 몬스터는 턴 넘김.
                TurnStart();
            else
                MonsterTurn(ObjectContext.Instance.Player, (Monster)actor);
        }
    }
    public bool TurnEnd()//턴 종료시 호출 (true: 전투 종료, false: 계속 진행)
    {
        for(int i = 0; i < MonsterList?.Count; i++)
        {
            OnMonsterDeath(MonsterList[i]);
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
    public void EndBattle()//전투 종료시 호출, 보상 지급.
    {
        ObjectContext.Instance.Player.GetExp(TotalExp);
        ObjectContext.Instance.Player.Gold += TotalGold;
        if(RewardItems != null)
        {
            foreach (var item in RewardItems)
            {
                ObjectContext.Instance.Shop.AddConsumItem(item);
            }
        }
    }
}
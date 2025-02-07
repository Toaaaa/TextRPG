using TextRPG.Objects;

namespace TextRPG;

public class Battle
{
    private int TotalExp { get; set; }
    private int TotalGold { get; set; }
    //public List<Item>? RewardItems { get; set; }


    PriorityQueue<Actor,int> TurnQueue = new PriorityQueue<Actor, int>();
    public List<Monster>? MonsterList { get; set; }

    //������ ����
    public int GetTotalExp()//���� ���� �� ȹ���� ����ġ ��ȯ.
    {
        return TotalExp;
    }
    public int GetTotalGold()//���� ���� �� ȹ���� ��� ��ȯ.
    {
        return TotalGold;
    }
    /*
    public List<Item> GetItems(List<Item> items)//���� ���� �� ȹ���� ������ ��ȯ.
    {
        return items;
    }
    */
    public string GetNowTurnName(string turn)//���� ������ ��ü �̸� ��ȯ.
    {
        return turn;
    }
    public bool GetIsPlayerTurn(bool turn)//�÷��̾��� ������ �ƴ��� ��ȯ. (true: �÷��̾� ��, false: ���� ��)
    {
        return turn;
    }   
    //�ൿ + ����
    public void PlayerAttack(Player player,Monster monster)//�÷��̾ ���� ���ý� ȣ��
    {

    }
    public void MonsterTurn()//������ �� ���ý� ȣ��
    {
        MonsterAttack();
    }
    void MonsterAttack()//���Ͱ� ���� ���ý� ȣ��
    {

    }
    void OnMonsterDeath(Monster monster)//���Ͱ� �׾��� �� ȣ��
    {
        TotalExp += monster.EXP;
        TotalGold += monster.Gold;
    }
    //����
    private Queue<Actor> GetTurnQueue(List<Actor> actors)//�� ���� ���ϱ�
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
        //���� ����Ʈ ��������
        TurnQueue = GetTurnQueue(actors);
    }
    private void StartBattle()
    {

    }

    // 1. ���� ������ ��� ��ü���� �ӵ����� ������ �� ������ ���Ѵ� (����� ����)
    // 2. �� ������ �������� ������ ���� ��ü���� �ൿ�� �����Ѵ�. (@@�� ��)
    // 3. �ൿ�� ������ ��ü���� �ൿ�� �����Ѵ�.
}
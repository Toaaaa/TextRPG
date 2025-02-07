
public enum PageType
{
    START_PAGE,
    STATUS_PAGE,
    INVENTORY_PAGE,
    BATTLE_PAGE,
    SHOP_PAGE,
}

public class Page
{
    private Router _router = null!;
    public Dictionary<PageType, Renderer> Scenes = new Dictionary<PageType, Renderer>();

    public void SetRouter(Router router)
    {
        this.Initialize();
        this._router = router;
    }

    private void Initialize()
    {
        Scenes = new Dictionary<PageType, Renderer>()
        {
            {
                PageType.START_PAGE,
                new Renderer()
                    .Contents(store =>
                    {
                        Console.WriteLine($"스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.");
                        Console.WriteLine("\n1. 상태 보기\n2. 인벤토리\n3.상점\n4. 던전입장\n");
                        Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
                    })
                    .Choices((store, selection) =>
                    {
                        switch (selection)
                        {
                            case 1:
                                _router.Navigate(PageType.STATUS_PAGE);
                                break;
                            case 2:
                                _router.Navigate(PageType.INVENTORY_PAGE);
                                break;
                            case 3:
                                _router.Navigate(PageType.SHOP_PAGE);
                                break;
                            case 4:
                                _router.Navigate(PageType.SHOP_PAGE);
                                break;
                        }
                    })
            },
            {
                PageType.INVENTORY_PAGE,
                new Renderer()
                    .Contents(store =>
                    {
                        Console.WriteLine($"인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]\n- [E]무쇠갑옷      | 방어력 +5 | 무쇠로 만들어져 튼튼한 갑옷입니다.\n- [E]스파르타의 창  | 공격력 +7 | 스파르타의 전사들이 사용했다는 전설의 창입니다.\n- 낡은 검         | 공격력 +2 | 쉽게 볼 수 있는 낡은 검 입니다.\n\n1. 장착 관리\n2. 나가기\n\n원하시는 행동을 입력해주세요.");
                    })
                    .Choices((store, selection) =>
                    {
                        switch (selection)
                        {
                            case 2:
                                _router.PopState();
                                break;
                        }
                    })
            },
            {
                PageType.STATUS_PAGE,
                new Renderer()
                    .Contents(store =>
                    {
                        Console.WriteLine($"상태 보기\n\n캐릭터의 정보가 표시됩니다.\n");
                        Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
                        Console.WriteLine("0. 나가기");
                    })
                    .Choices((store, selection) =>
                    {

                        switch (selection)
                        {
                            case 0:
                                _router.PopState();
                                break;
                        }
                    })
            },
            {
                PageType.BATTLE_PAGE,
                new Renderer()
                    .Contents(store =>
                    {
                        var battleMode = store.State<string>("BATTLE_MODE").Init("DEFAULT");
                        
                        Console.WriteLine($"Battle!!\n\nLv.2 미니언  HP 15\nLv.5 대포미니언 HP 25\nLV.3 공허충 HP 10\n\n\n[내정보]\nLv.1  Chad (전사) \nHP 100/100 \n\n1. 공격");
                        
                        if(battleMode.GetValue() == "DEFAULT") Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
                        if(battleMode.GetValue() == "BATTLE_MODE") Console.WriteLine("\n대상을 선택해주세요.\n>>");
                    })
                    .Choices((store, selection) =>
                    {
                        var battleMode = store.State<string>("BATTLE_MODE");

                        switch (selection)
                        {
                            case 0: 
                                _router.PopState();
                                break;
                            case 1:
                                battleMode.SetValue("BATTLE_MODE");
                                break;
                        }
                    })
            },
            {
                PageType.SHOP_PAGE,
                new Renderer()
                    .Contents(store =>
                    {
                        Console.WriteLine($"상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n\n[보유 골드]\n800 G\n\n[아이템 목록]\n- 수련자 갑옷    | 방어력 +5  | 수련에 도움을 주는 갑옷입니다.             |  1000 G\n- 무쇠갑옷      | 방어력 +9  | 무쇠로 만들어져 튼튼한 갑옷입니다.           |  구매완료\n- 스파르타의 갑옷 | 방어력 +15 | 스파르타의 전사들이 사용했다는 전설의 갑옷입니다.|  3500 G\n- 낡은 검      | 공격력 +2  | 쉽게 볼 수 있는 낡은 검 입니다.            |  600 G\n- 청동 도끼     | 공격력 +5  |  어디선가 사용됐던거 같은 도끼입니다.        |  1500 G\n- 스파르타의 창  | 공격력 +7  | 스파르타의 전사들이 사용했다는 전설의 창입니다. |  구매완료\n\n1. 아이템 구매\n0. 나가기\n\n원하시는 행동을 입력해주세요.\n>>");
                    })
                    .Choices((store, selection) =>
                    {
                        switch (selection)
                        {
                            case 0:
                                _router.PopState();
                                break;
                            case 1:
                                break;
                        }
                    })
            }
        };    
    }   
}

public enum PageType
{
    START_PAGE,
    STATUS_PAGE,
    BATTLE_PAGE,
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
                        Console.WriteLine("\n1. 상태 보기\n2. 전투 시작");
                        Console.WriteLine("\n원하시는 행동을 입력해주세요.\n>>");
                    })
                    .Choices((store, select) =>
                    {
                        switch (select)
                        {
                            case 1:
                                _router.Navigate(PageType.STATUS_PAGE);
                                break;
                            case 2:
                                _router.Navigate(PageType.BATTLE_PAGE);
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
                    .Choices((store, select) =>
                    {

                        switch (select)
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
                    .Choices((store, select) =>
                    {
                        var battleMode = store.State<string>("BATTLE_MODE");

                        switch (select)
                        {
                            case 0: 
                                _router.PopState();
                                break;
                            case 1:
                                battleMode.SetValue("BATTLE_MODE");
                                break;
                        }
                    })
            }
        };    
    }   
}
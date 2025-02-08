
using TextRPG;
using TextRPG.Objects;
using TextRPG.Objects.Items;
using TextRPG.Objects.Shop;

public enum PageType
{
    INIT_PAGE,
    START_PAGE,
    STATUS_PAGE,
    INVENTORY_PAGE,
    SHOP_PAGE,
    SHOP_TRADE_PAGE,
    DUNGEON_PAGE,
    BATTLE_PAGE,
    REWARD_PAGE,
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
                PageType.INIT_PAGE,
                new Renderer((context, states) =>
                {
                    var mode = states.Get<string>("MODE").Init("NAME");
                    Player player = ObjectContext.Instance.Player;
                    
                    context.Content = () =>
                    {
                        //skip
                        _router.Navigate(PageType.START_PAGE);

                        Console.WriteLine($"스파르타 던전에 오신 여러분 환영합니다.");
                        if(mode.GetValue() == "NAME") Console.WriteLine($"원하시는 이름을 설정해주세요.");
                        if (mode.GetValue() == "CLASS")
                        {
                            Console.WriteLine($"{player.Name}님 원하시는 직업을 선택해주세요.\n");
                            Console.WriteLine("1.전사\n2.궁수\n3.도적\n4.마법사");
                        }
                    };
                    context.Choice = () =>
                    {
                        if (mode.GetValue() == "NAME")
                        {
                            if(string.IsNullOrWhiteSpace(context.Selection)) context.Error();
                            else
                            {
                                player.Name = context.Selection!.ToString();
                                mode.SetValue("CLASS");
                                return;
                            }
                        }

                        if (mode.GetValue() == "CLASS")
                        {
                            switch (context.Selection)
                            {
                                case 1:
                                    player.Class = "전사";
                                    break;
                                case 2:
                                    player.Class = "궁수";
                                    break;
                                case 3:
                                    player.Class = "도적";
                                    break;
                                case 4:
                                    player.Class = "마법사";
                                    break;
                                default:
                                    context.Error();
                                    break;
                            }
                            _router.Navigate(PageType.START_PAGE);
                        }
                    };
                })
            },
            {
                PageType.START_PAGE,
                new Renderer((context, states) =>
                {
                    context.Content = () =>
                    {
                        Console.WriteLine($"스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.");
                        Console.WriteLine("\n1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n");
                        Console.WriteLine("원하시는 행동을 입력해주세요. >>");
                    };
                    
                    context.Choice = () =>
                    {
                        switch (context.Selection)
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
                                _router.Navigate(PageType.DUNGEON_PAGE);
                                break;
                            default:
                                context.Error();
                                break;
                        }
                    };
                })
            },
            {
                PageType.STATUS_PAGE,
                new Renderer((context, states) =>
                    {
                        Player player = ObjectContext.Instance.Player;
            
                        context.Content = () =>
                        {
                            Console.WriteLine($"상태 보기\n\n캐릭터의 정보가 표시됩니다.\n");
                            Console.WriteLine($"Lv. {player.Name}\nChad ( {player.Class} )\n공격력 : {player.TotalATK}\n방어력 : {player.TotalDEF}\n체 력 : {player.MaxHP}\nGold : {player.Gold} G\n");
                            Console.WriteLine("0. 나가기");
                            Console.WriteLine("\n원하시는 행동을 입력해주세요. >>");
                        };
            
                        context.Choice = () =>
                        {
                            switch (context.Selection)
                            {
                                case 0:
                                    _router.PopState();
                                    break;
                                default:
                                    context.Error();
                                    break;
                            }
                        };
                    })
            },
            {
                PageType.INVENTORY_PAGE,
                new Renderer((context, states) =>
                {
                    List<Item> inventory = ObjectContext.Instance.Player.Inventory;
                    var mode = states.Get<string>("MODE").Init("VIEW");

                    context.Content = () =>
                    {
                        Console.WriteLine($"인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]");
                        for (int i = 0; i < inventory.Count; i++)
                        {
                            Item item = inventory[i];
                            Console.WriteLine($"{i + 1}. {item.Name} | {item.Explain} | {item.Price}");
                        }
                        Console.WriteLine("\n\n1. 장착 관리\n2. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    
                    context.Choice = () =>
                    {
                        switch (context.Selection)
                        {
                            case 1:
                                mode.SetValue("EQUIPMENT");
                                break;
                            case 2:
                                _router.PopState();
                                break;
                            default:
                                context.Error();
                                break;
                        }
                    };
                })
            },
            {
                PageType.SHOP_PAGE,
                new Renderer((context, states) =>
                {
                    Shop shop = ObjectContext.Instance.Shop;
                    Player player = ObjectContext.Instance.Player;
                    var itemsList = new List<Item>(shop.AllItem.Values);

                    var mode = states.Get<string>("MODE").Init("VIEW");

                    context.Content = () =>
                    {
                        Console.WriteLine($"상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n\n[보유 골드]\n{player.Gold} G\n\n");
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            var item = itemsList[i];
                            Console.WriteLine($"{i}. {item.Name}");
                        }
                        Console.WriteLine("\n\n1. 구매하기\n2. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    
                    context.Choice = () =>
                    {
                        switch (context.Selection)
                        {
                            case 1:
                                mode.SetValue("BUY");
                                break;
                            case 2:
                                _router.PopState();
                                break;
                            default:
                                context.Error();
                                break;
                        }
                    };
                })
            },
            {
                PageType.DUNGEON_PAGE,
                new Renderer((context, states) =>
                {
                    Dungeon dungeon = ObjectContext.Instance.Dungeon;
                    Battle battle = ObjectContext.Instance.Battle;

                    context.Content = () =>
                    {
                        Console.WriteLine("던전 입장\n던전을 선택해주세요.\n");
                        for (int index = 0; index < dungeon.stages.Count; index++)
                        {
                            Console.WriteLine($"{index + 1}. {dungeon.stages[index].Name}");      
                        }
                    };
                    
                    context.Choice = () =>
                    {
                        if (context.Selection is not int || context.Selection < 1 || context.Selection > dungeon.stages.Count)
                        {
                            context.Error(); 
                            return;
                        }
                        dungeon.EnterStage(context.Selection);
                        battle.BeforeBattle();
                        _router.Navigate(PageType.BATTLE_PAGE);
                    };
                })
            },
            {
                PageType.BATTLE_PAGE,
                new Renderer((context, states) =>
                    {
                        Battle battle = ObjectContext.Instance.Battle;

                        Player player = ObjectContext.Instance.Player;
                        Dungeon dungeon = ObjectContext.Instance.Dungeon;
                        List<Monster> monsters = dungeon.GetMonsterList();
                        
                        context.Content = () =>
                        {
                            Console.WriteLine($"Battle!!");

                            for (int index = 0; index < monsters.Count; index++)
                            {
                                Monster monster = monsters[index];
                                Console.WriteLine($"Lv.{monster.Level} {monster.Name} - HP {monster.HP}");
                            }

                        };
                    })
            },
        };    
    }   
}

using System.Collections;
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
                        
                        if(mode.GetValue() == "NAME") 
                            Console.WriteLine($"원하시는 이름을 설정해주세요.");
                        
                        if (mode.GetValue() == "CLASS") 
                            Console.WriteLine(
                                $"{player.Name}님 원하시는 직업을 선택해주세요.\n\n" +
                                $"1.전사\n2.궁수\n3.도적\n4.마법사");
                    };
                    context.Choice = () =>
                    {
                        if (mode.GetValue() == "NAME")
                        {
                            if(string.IsNullOrWhiteSpace(context.SelectionText)) context.Error();
                            else
                            {
                                player.Name = context.SelectionText;
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
                        Console.WriteLine(
                            $"스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.\n\n" +
                            $"1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n\n" +
                            $"원하시는 행동을 입력해주세요. >>");
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
                            Console.WriteLine(
                                $"상태 보기\n" +
                                $"캐릭터의 정보가 표시됩니다.\n\n" +
                                $"Lv. {player.Level}\n" +
                                $"Chad ( {player.Class} )\n" +
                                $"이름 : {player.Name}\n" +
                                $"공격력 : {player.TotalATK}\n" +
                                $"방어력 : {player.TotalDEF}\n" +
                                $"체 력 : {player.MaxHP}\n" +
                                $"Gold : {player.Gold} G\n\n" +
                                $"0. 나가기\n\n" +
                                $"원하시는 행동을 입력해주세요. >>\n");
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
                    Player player = ObjectContext.Instance.Player;
                    var equipments = player.Inventory.OfType<EquipItem>();
                    
                    var mode = states.Get<string>("MODE").Init("VIEW");

                    context.Content = () =>
                    {
                        Console.WriteLine(
                            $"인벤토리\n" +
                            $"보유 중인 아이템을 관리할 수 있습니다.\n\n" +
                            $"[아이템 목록]");
                        
                        for (int i = 0; i < equipments.Count(); i++)
                        {
                            EquipItem item = equipments.ElementAt(i);
                            
                            Console.ForegroundColor = item.IsEquip ? ConsoleColor.Blue : ConsoleColor.Gray; 
                        
                            if(item.IsEquip) Console.Write($"[E] ");
                            if(mode.GetValue() == "EQUIPMENT") Console.Write($"{i + 1}. ");
                            Console.WriteLine($"{item.Name} | {item.Explain} | {item.Price}G");
                        }
                        Console.ResetColor();

                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                Console.WriteLine(
                                    "\n0. 나가기\n1. 장착 관리\n\n" +
                                    "원하시는 행동을 입력해주세요. >>");
                                break;
                            case "EQUIPMENT":
                                Console.WriteLine(
                                    "\n0. 나가기\n\n" +
                                    "원하시는 행동을 입력해주세요. >>");
                                break;
                        }
                    };
                    
                    context.Choice = () =>
                    {
                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                switch (context.Selection)
                                {
                                    case 0:
                                        _router.PopState();
                                        break;
                                    case 1:
                                        mode.SetValue("EQUIPMENT");
                                        break;
                                    default:
                                        context.Error();
                                        break;
                                }
                                break;
                            case "EQUIPMENT":
                                if (context.Selection == 0)
                                {
                                    mode.SetValue("VIEW");
                                    break;
                                }

                                if (context.Selection > equipments.Count())
                                {
                                    context.Error();
                                    break;
                                }
                                EquipItem equipItem = equipments.ElementAt(context.Selection - 1);
                                if(equipItem.IsEquip) player.UnequipItem(equipItem);
                                else player.EquipItem(equipItem);
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

                    var equipItems = shop.EquipItemShop;
                    var consumItems = shop.ConsumItemShop;
                    
                    var mode = states.Get<string>("MODE").Init("VIEW");
                    var category = states.Get<string>("CATEGORY").Init("NONE");
                    var result = states.Get<TradeResult>("RESULT").Init(TradeResult.None);
                    
                    context.Content = () =>
                    {
                        Console.WriteLine(
                            $"상점\n" +
                            $"필요한 아이템을 얻을 수 있는 상점입니다.\n\n" +
                            $"[보유 골드]\n{player.Gold} G\n");

                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                {
                                    Console.WriteLine(
                                        "0. 나가기\n1. 구매하기\n\n" +
                                        "원하시는 행동을 입력해주세요. >>");
                                    break;
                                }
                            case "CATEGORY":
                                {
                                    Console.WriteLine(
                                        "0.나가기\n1. 장비\n2. 소모품\n\n" +
                                        "원하시는 행동을 입력해주세요. >>");
                                }
                                break;
                            case "BUYING":
                                switch (category.GetValue())
                                {
                                    case "EQUIPMENT":
                                        for (int i = 0; i < equipItems.Count; i++)
                                        {
                                            Item item = (Item)equipItems[i];
                                            Console.WriteLine($"{i + 1}. { item.Name} | {item.Explain} | {item.Price}G");
                                        }
                                        
                                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("구매를 성공했습니다.", ConsoleColor.Green);
                                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("이미 구입한 상품입니다.", ConsoleColor.Red);
                                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("골드가 부족합니다.", ConsoleColor.Red);
                                        
                                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                                        break;
                                    case "CONSUM":
                                        for (int i = 0; i < consumItems.Count; i++)
                                        {
                                            Item item = (Item)consumItems[i];
                                            Console.WriteLine($"{i + 1}. { item.Name} | {item.Explain} | {item.Price}G");
                                        }
                                        
                                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("구매를 성공했습니다.", ConsoleColor.Green);
                                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("이미 구입한 상품입니다.", ConsoleColor.Red);
                                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("골드가 부족합니다.", ConsoleColor.Red);
                                        
                                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                                        break;
                                }
                                break;
                        }
                    };
                    
                    context.Choice = () =>
                    {
                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                switch (context.Selection)
                                {
                                    case 0:
                                        _router.PopState();
                                        break;
                                    case 1:
                                        mode.SetValue("CATEGORY");
                                        break;
                                    default:
                                        context.Error();
                                        break;
                                }
                                break;
                            case "CATEGORY":
                                switch (context.Selection)
                                {
                                    case 0:
                                        mode.SetValue("VIEW");
                                        break;
                                    case 1:
                                        mode.SetValue("BUYING");
                                        category.SetValue("EQUIPMENT");                                        
                                        break;
                                    case 2:
                                        mode.SetValue("BUYING");
                                        category.SetValue("CONSUM");                                 
                                        break;
                                    default:
                                        context.Error();
                                        break;
                                }
                                break;
                            case "BUYING":
                                if (context.Selection == 0)
                                {
                                    mode.SetValue("CATEGORY");
                                    category.SetValue("NONE");
                                    result.SetValue(TradeResult.None);
                                    break;
                                }
                                switch (category.GetValue())
                                {
                                     case "EQUIPMENT":
                                         if (context.Selection > equipItems.Count)
                                         {
                                             context.Error();
                                             break;
                                         }
                                         result.SetValue(shop.BuyEquipItem(context.Selection));
                                         break;
                                     case "CONSUM":
                                         if (context.Selection > consumItems.Count)
                                         {
                                             context.Error();
                                             break;
                                         }
                                         result.SetValue(shop.BuyConsumItem(context.Selection));
                                         break;
                                }
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
                        Console.WriteLine(
                            "던전 입장\n" +
                            "던전을 선택해주세요.\n");
                        
                        for (int index = 0; index < dungeon.stages.Count; index++)
                        {
                            Console.WriteLine($"{index + 1}. {dungeon.stages[index].Name}");      
                        }
                        Console.WriteLine($"\n0.나가기");
                    };
                    
                    context.Choice = () =>
                    {
                        if (context.Selection == 0)
                        {
                            _router.PopState();
                            return;
                        }
                        
                        if (context.Selection < 1 || context.Selection > dungeon.stages.Count)
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
                        Player player = ObjectContext.Instance.Player;
                        Dungeon dungeon = ObjectContext.Instance.Dungeon;
                        Battle battle = ObjectContext.Instance.Battle;

                        var mode = states.Get<string>("MODE").Init("WAITING");
                        var turn = states.Get<bool?>("TURN").Init(null);
                        var isEnd = states.Get<bool?>("END").Init(false);
                        
                        context.Content = () =>
                        {
                            // if (turn.GetValue() == null) turn.SetValue(battle.GetIsPlayerTurn());
                            if (turn.GetValue() == null) turn.SetValue(true);

                            Logger.WriteLine($"Battle!!\n", ConsoleColor.Yellow);
                            
                            switch (turn.GetValue())
                            {
                                case true:
                                    switch (mode.GetValue())
                                    {
                                        case "WAITING":
                                        case "SELECT":
                                            {
                                                // show monster list
                                                for (int index = 0; index < dungeon.MonsterList.Count; index++)
                                                {
                                                    Monster monster = dungeon.MonsterList[index];
                                                    if (mode.GetValue() == "SELECT") Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                                    Console.Write($"Lv.{monster.Level} {monster.Name} - ");
                                                    Console.WriteLine(monster.IsDead ? "Dead" : $"HP {monster.HP}");
                                                }

                                                Console.WriteLine(
                                                    $"\n[내정보]\n" +
                                                    $"Lv.{player.Level}  Chad ({player.Class}) \n" +
                                                    $"HP {player.HP}/{player.MaxHP}\n");

                                                if (mode.GetValue() == "WAITING") Console.WriteLine("1. 공격");
                                                else if (mode.GetValue() == "SELECT") Console.WriteLine("0. 취소");

                                                break;
                                            }
                                        case "SELECT_END":
                                            {
                                                Monster monster = battle.TargetMonster;
                                                Console.WriteLine(
                                                    $"{player.Name} 의 공격!\n" +
                                                    $"Lv.{monster.Level} {monster.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                                    $"Lv.{monster.Level} {monster.Name}\nHP {monster.MaxHP} -> {monster.HP}\n\n" +
                                                    $"0. 다음");
                                            }
                                            break;
                                    }
                                    break;
                                case false:
                                    {
                                        Actor monster = battle.CurrentActor;
                                        Console.WriteLine(
                                            $"{monster.Name} 의 공격!\n" +
                                            $"{player.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                            $"Lv.{player.Level} {player.Name}\nHP {player.MaxHP} -> {player.HP}\n\n" +
                                            $"0. 다음");
                                    }
                                    break;
                            }
                        };

                        context.Choice = () =>
                        {
                            // 배틀 종료 확인을 최상단에서 체크
                            switch (turn.GetValue())
                            {
                                case true:
                                    switch (mode.GetValue())
                                    {
                                        case "WAITING":
                                                if (context.Selection != 1) { context.Error(); return; }
                                                mode.SetValue("SELECT");
                                            break;
                                        case "SELECT":
                                            {
                                                if(context.Selection == 0) { mode.SetValue("WAITING"); return; }
                                                if (context.Selection > dungeon.MonsterList.Count) { context.Error(); return; }

                                                battle.SetTargetMonster(context.Selection - 1);
                                                mode.SetValue("SELECT_END");
                                                
                                                turn.SetValue(battle.GetIsPlayerTurn());
                                                battle.TurnStart();
                                                break;
                                            }
                                        case "SELECT_END":
                                            {
                                                if(context.Selection != 0) { context.Error(); return; }
                                                // 공통 로직
                                                // 한 사이클이 끝나 선택 화면으로
                                                if (battle.TurnQueue.Count == 0)
                                                {
                                                    turn.SetValue(true);
                                                    mode.SetValue("WAITING");
                                                }
                                                else
                                                {
                                                    turn.SetValue(battle.GetIsPlayerTurn());
                                                }
                                                bool isBattleEnd = battle.BattleTurn();
                                                if (isBattleEnd)
                                                {
                                                    _router.Navigate(PageType.REWARD_PAGE);
                                                    break;
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                // queue가 1개 남았을 때 턴 확인 시 오류 발생
                                case false:
                                    {
                                        if(context.Selection != 0) { context.Error(); return; }
                                        // 공통 로직
                                        if (battle.TurnQueue.Count == 0)
                                        {
                                            turn.SetValue(true);
                                            mode.SetValue("WAITING");
                                        }
                                        else
                                        {
                                            turn.SetValue(battle.GetIsPlayerTurn());
                                        }
                                        bool isBattleEnd = battle.BattleTurn();
                                        if (isBattleEnd)
                                        {
                                            _router.Navigate(PageType.REWARD_PAGE);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        };
                    })
            },
        };    
    }   
}
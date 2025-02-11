
using System.Collections;
using TextRPG;
using TextRPG.Objects;
using TextRPG.Objects.Items;
using TextRPG.Objects.Shop;
using TextRPG.Objects.Smith;

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
    SMITHY_PAGE
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
                    string[] classes = ["전사", "궁수", "도적", "마법사"];

                    context.Content = () =>
                    {
                        //skip
                        _router.Navigate(PageType.START_PAGE);

                        Console.WriteLine($"스파르타 던전에 오신 여러분 환영합니다.");

                        if (mode.GetValue() == "NAME")
                        {
                            context.SelectionMode = Renderer.SelectionType.text;
                            Console.WriteLine($"원하시는 이름을 설정해주세요.");
                        }

                        if (mode.GetValue() == "CLASS")
                        {
                            context.SelectionMode = Renderer.SelectionType.number;
                            Console.WriteLine(
                                $"{player.Name}님 원하시는 직업을 선택해주세요.\n\n" +
                                $"1.전사\n2.궁수\n3.도적\n4.마법사");
                        }
                           
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
                            if (context.Selection < 0 || context.Selection > classes.Length) { context.Error(); return; }
                            player.Class = classes[context.Selection - 1];
                            _router.Navigate(PageType.START_PAGE);
                        }
                    };
                })
            },
            {
                PageType.START_PAGE,
                new Renderer((context, states) =>
                {
                    PageType[] pageTypes = [PageType.STATUS_PAGE, PageType.INVENTORY_PAGE, PageType.SHOP_PAGE, PageType.SMITHY_PAGE, PageType.DUNGEON_PAGE];

                    context.Content = () =>
                    {
                        Console.WriteLine(
                            $"스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.\n\n" +
                            $"1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 강화소\n5. 던전입장\n\n" +
                            $"원하시는 행동을 입력해주세요. >>");
                    };
                    
                    context.Choice = () =>
                    {
                        if (context.Selection < 1 || context.Selection > pageTypes.Length) { context.Error(); return; }
                        _router.Navigate(pageTypes[context.Selection - 1]);
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
                                $"체 력 : {player.HP}/{player.MaxHP}\n" +
                                $"마 력 : {player.MP}/{player.MaxMP}\n" +
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
                        Console.WriteLine($"인벤토리\n" + $"보유 중인 아이템을 관리할 수 있습니다.\n\n" + $"[아이템 목록]");
                        
                        for (int i = 0; i < equipments.Count(); i++)
                        {
                            EquipItem item = equipments.ElementAt(i);
                            
                            Console.ForegroundColor = item.IsEquip ? ConsoleColor.Blue : ConsoleColor.Gray; 
                        
                            if(item.IsEquip) Console.Write($"[E] ");
                            if(mode.GetValue() == "EQUIPMENT") Console.Write($"{i + 1}. ");
                            Console.WriteLine($"{item.Name} | {item.Explain} | +{item.Stat}");
                        }
                        Console.ResetColor();

                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                Console.WriteLine("\n0. 나가기\n1. 장착 관리\n\n" + "원하시는 행동을 입력해주세요. >>");
                                break;
                            case "EQUIPMENT":
                                Console.WriteLine("\n0. 나가기\n\n" + "원하시는 행동을 입력해주세요. >>");
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
                                if (context.Selection == 0) { mode.SetValue("VIEW"); break; }
                                if (context.Selection > equipments.Count()) { context.Error(); break; }
                                
                                // 장착, 미장착
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

                    var inventory = player.Inventory;
                    var equipItems = shop.EquipItemShop;
                    var consumItems = shop.ConsumItemShop;
                    
                    var mode = states.Get<string>("MODE").Init("VIEW");
                    var category = states.Get<string>("CATEGORY").Init("NONE");
                    var result = states.Get<TradeResult>("RESULT").Init(TradeResult.None);
                    
                    context.Content = () =>
                    {
                        Console.WriteLine($"상점\n" + $"필요한 아이템을 얻을 수 있는 상점입니다.\n\n" + $"[보유 골드]\n{player.Gold} G\n");

                        switch (mode.GetValue())
                        {
                            case "VIEW":
                                Console.WriteLine("0. 나가기\n1. 구매하기\n2. 판매하기\n\n" + "원하시는 행동을 입력해주세요. >>");
                                break;
                            
                            case "CATEGORY":
                                Console.WriteLine("0. 나가기\n1. 장비\n2. 소모품\n\n" + "원하시는 행동을 입력해주세요. >>");
                                break;
                            
                            case "BUYING":
                                switch (category.GetValue())
                                {
                                    case "EQUIPMENT":
                                        for (int i = 0; i < equipItems.Count; i++)
                                        {
                                            Item item = (Item)equipItems[i];
                                            bool isExistItem = inventory.Contains(item!);
                                            Logger.WriteLine($"{i + 1}. { item.Name} | {item.Explain} | {(isExistItem? "구매 완료" : item.Price +"G")}", isExistItem ? ConsoleColor.DarkCyan: ConsoleColor.Gray);
                                        }
                                        
                                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("\n구매를 성공했습니다.", ConsoleColor.Green);
                                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("\n이미 구입한 상품입니다.", ConsoleColor.Red);
                                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("\n골드가 부족합니다.", ConsoleColor.Red);
                                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                                        break;
                                    
                                    case "CONSUM":
                                        for (int i = 0; i < consumItems.Count; i++)
                                        {
                                            Item item = (Item)consumItems[i];
                                            Console.WriteLine($"{i + 1}. { item.Name} | {item.Explain} | {item.Price}G");
                                        }
                                        
                                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("\n구매를 성공했습니다.", ConsoleColor.Green);
                                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("\n이미 구입한 상품입니다.", ConsoleColor.Red);
                                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("\n골드가 부족합니다.", ConsoleColor.Red);
                                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                                        break;
                                }
                                break;
                            case "SELLING":
                                {
                                    for (int i = 0; i < inventory.Count; i++)
                                    {
                                        Item item = inventory[i];
                                        Console.WriteLine($"{i + 1}. {item.Name} | {item.Explain} | {item.Price}G");
                                    }
                                    
                                    Console.WriteLine("\n0. 나가기\n\n" + "원하시는 행동을 입력해주세요. >>");
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
                                    case 2:
                                        mode.SetValue("SELLING");
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
                                if (context.Selection == 0) { mode.SetValue("CATEGORY"); category.SetValue("NONE"); result.SetValue(TradeResult.None); break; }
                                switch (category.GetValue())
                                {
                                     case "EQUIPMENT":
                                         if (context.Selection > equipItems.Count) { context.Error(); break; }
                                         result.SetValue(shop.BuyEquipItem(context.Selection));
                                         break;
                                     
                                     case "CONSUM":
                                         if (context.Selection > consumItems.Count) { context.Error(); break; }
                                         result.SetValue(shop.BuyConsumItem(context.Selection));
                                         break;
                                }
                                break;
                            case "SELLING":
                                if (context.Selection == 0) { mode.SetValue("VIEW"); result.SetValue(TradeResult.None); break; }
                                if (context.Selection > inventory.Count) { context.Error(); break; }
                                
                                shop.SellItem(context.Selection - 1);
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
                        Console.WriteLine("던전 입장\n" + "던전을 선택해주세요.\n");
                        
                        for (int index = 0; index < dungeon.stages.Count; index++)
                        {
                            Console.WriteLine($"{index + 1}. {dungeon.stages[index].Name}");      
                        }
                        Console.WriteLine($"\n0.나가기");
                    };
                    
                    context.Choice = () =>
                    {
                        if (context.Selection == 0) { _router.PopState(); return; }
                        if (context.Selection < 1 || context.Selection > dungeon.stages.Count) { context.Error(); return; }
                        
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
                        // 맨 처음은 플레이어 턴으로 적용
                        var isPlayerTurn = states.Get<bool?>("TURN").Init(true);
                        var cycle = states.Get<int?>("CYCLE").Init(0);
                        
                        // 배틀 관련 
                        int GetCurrentMaxTurnCycle() => battle.MonsterList!.FindAll(monster => !monster.IsDead).Count + 1;
                        void RefillTurnCycle() {
                            // 현재 남은 몬스터를 기준으로 queue 재할당
                            List<Actor> currentActors = new List<Actor>() {};
                            currentActors.AddRange(battle.MonsterList!.FindAll(monster => !monster.IsDead));
                            currentActors.Add(player);
                            battle.TurnQueue = battle.GetTurnQueue(currentActors);
                                                
                            // 만일 사이클이 끝난 뒤 다시 선택 화면을 우선 처리하고 싶다면 분리 종료
                            isPlayerTurn.SetValue(true);
                            cycle.SetValue(0);
                        }
                        
                        var consumItems = player.Inventory.OfType<ConsumItem>();
                        var selectedSKill = states.Get<Skill?>("SELECTED_SKILL").Init(null);
                        var selectedItem = states.Get<ConsumItem?>("SELECTED_ITEM").Init(null);
                        // do: 한번만 호출 필요
                        Dictionary<string, Skill> skills = Skill.LoadSkillDictionary(Path.Combine(Path.GetFullPath(@"../../../Objects/SkillList.json")));

                            
                        context.Content = () =>
                        {

                            Logger.WriteLine($"Battle!!\n", ConsoleColor.Yellow);
                            
                            switch (isPlayerTurn.GetValue())
                            {
                                case true:
                                    switch (mode.GetValue())
                                    {
                                        case "WAITING":
                                        case "CHOOSE_TARGET":
                                        case "SELECT_SKILL":
                                        case "USING_ITEM":
                                            {
                                                // show monster list
                                                for (int index = 0; index < dungeon.MonsterList.Count; index++)
                                                {
                                                    Monster monster = dungeon.MonsterList[index];
                                                    if (mode.GetValue() == "CHOOSE_TARGET") Logger.Write($"{index + 1} ", ConsoleColor.Cyan);

                                                    // 죽은 몬스터는 아예 선택이 안되도록 처리해도 좋을 듯.
                                                    if (monster.IsDead) Console.ForegroundColor = ConsoleColor.DarkGray;

                                                    Console.Write($"Lv.{monster.Level} {monster.Name} - ");
                                                    Console.WriteLine(monster.IsDead ? "Dead" : $"HP {monster.HP}");
                                                    
                                                    Console.ResetColor();
                                                }

                                                Console.WriteLine($"\n[내정보]\n" + $"Lv.{player.Level}  Chad ({player.Class}) \n" + $"HP {player.HP}/{player.MaxHP}\n");

                                                switch (mode.GetValue())
                                                {
                                                    case "WAITING":
                                                        Console.WriteLine("0. 나가기\n1. 공격\n2. 스킬\n3. 아이템");
                                                        break;
                                                    case "CHOOSE_TARGET":
                                                        Console.WriteLine("0. 취소");
                                                        break;
                                                    case "SELECT_SKILL":
                                                        for (int index = 0; index < skills.Count(); index++)
                                                        {
                                                            Skill currentSkill = skills.ElementAt(index).Value; 
                                                            Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                                            Console.WriteLine($"{currentSkill.Name} | {currentSkill.Explain} | {currentSkill.Mana}MP");
                                                        }
                                                        
                                                        Console.WriteLine("\n0. 취소");
                                                        break;
                                                    
                                                    case "USING_ITEM":
                                                        IEnumerable<ConsumItem> items = player.Inventory.OfType<ConsumItem>();
                                                        for (int index = 0; index < items.Count(); index++)
                                                        {
                                                            ConsumItem item = items.ElementAt(index);
                                                            Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                                            // 아이템 갯수 확인 필요
                                                            Console.WriteLine($"{item.Name} | {item.Explain}");
                                                        }
                                                        Console.WriteLine("\n0. 취소");
                                                        break;
                                                }

                                                break;
                                            }
                                        case "SELECT_DONE":
                                                // 아이템 사용인 경우
                                                if (selectedItem.GetValue() != null)
                                                {
                                                    Console.WriteLine($"{player.Name} 가 {selectedItem.GetValue().Name}을 사용했습니다.\n" + $"HP {player.HP} -> {player.HP}\n\n" + $"HP {player.MP} -> {player.MP}\n\n" + $"0. 다음");
                                                    break;
                                                }

                                                if (selectedSKill.GetValue() != null)
                                                {
                                                    foreach (Actor monster in battle.Target)
                                                    {
                                                        // 체력이 0 일때 값이 달라짐
                                                        Console.WriteLine(
                                                            $"{player.Name}가 {selectedSKill.GetValue().Name} 스킬을 사용했습니다.\n" + $"Lv.{monster.Level} {monster.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                                            $"Lv.{monster.Level} {monster.Name}\n" + $"HP {monster.HP + battle.LastDamage} -> {monster.HP}\n");
                                                    }
                                                    Console.WriteLine($"\n0. 다음");
                                                    break;
                                                }
 
                                                foreach (Actor monster in battle.Target)
                                                {
                                                    Console.WriteLine(
                                                        $"{player.Name} 의 공격!\n" + $"Lv.{monster.Level} {monster.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                                        $"Lv.{monster.Level} {monster.Name}\n" + $"HP {monster.HP + battle.LastDamage} -> {monster.HP}\n\n");
                                                }
                                                Console.WriteLine($"\n0. 다음");
                                                
                                            break;
                                    }
                                    break;
                                case false:
                                    {
                                        Actor monster = battle.CurrentActor;
                                        Console.WriteLine(
                                            $"{monster.Name} 의 공격!\n" + $"{player.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                            $"Lv.{player.Level} {player.Name}\nHP {player.HP + battle.LastDamage} -> {player.HP}\n\n" + $"0. 다음");
                                    }
                                    break;
                            }
                        };

                        context.Choice = () =>
                        {
                            // 배틀 종료 확인을 최상단에서 체크
                            switch (isPlayerTurn.GetValue())
                            {
                                case true:
                                    switch (mode.GetValue())
                                    {
                                        case "WAITING":
                                            string[] modes = ["CHOOSE_TARGET", "SELECT_SKILL", "USING_ITEM"];

                                            if(context.Selection == 0) { _router.PopState();; return; }
                                            if(context.Selection > modes.Length) { context.Error(); return; }
                                            mode.SetValue(modes[context.Selection - 1]);
                                            
                                            break;
                                        
                                        case "SELECT_SKILL":
                                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }                                           
                                            if (context.Selection > skills.Count()) { context.Error(); return; }
                                            
                                            Skill currentSkill = skills.ElementAt(context.Selection - 1).Value;
                                            
                                            if(currentSkill.Mana > player.MP) { context.Error("마나가 부족합니다."); return; }
                                            selectedSKill.SetValue(currentSkill);

                                            mode.SetValue("CHOOSE_TARGET");
                                            break;
                                        
                                        case "CHOOSE_TARGET":
                                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }
                                            if (context.Selection > dungeon.MonsterList.Count) { context.Error(); return; }
                                            if ((bool)battle.GetMonsterIsDead(context.Selection - 1)) { context.Error(); break; } // 죽은 몬스터를 선택한 경우

                                            // 대상 지정, 플레이어 행동 결정 완료
                                            battle.SetTargetMonster([battle.MonsterList[context.Selection - 1]]);
                                            
                                            
                                            if (selectedSKill.GetValue() == null) { Battle.PlayerAction = () => battle.Target.ForEach(target => battle.PlayerAttack((target as Monster)!)); }
                                            else { Battle.PlayerAction = () => battle.Target.ForEach(target => battle.PlayerSkillAttack(target as Monster, selectedSKill.GetValue())); }
                                            
                                            // 다음 턴 진행
                                            mode.SetValue("SELECT_DONE");
                                            isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                                            battle.TurnStart();
                                            cycle.SetValue(prev => prev + 1);
                                            break;
                                      
                                        case "USING_ITEM":
                                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }
                                            if (context.Selection > consumItems.Count() + 1) { context.Error(); break; }
                                            
                                            selectedItem.SetValue(consumItems.ElementAt(context.Selection - 1));
                                            Battle.PlayerAction = () => selectedItem.GetValue().UseItem(player, EConsumItem.Potion);
                                            
                                            // 다음 턴 진행
                                            mode.SetValue("SELECT_DONE");
                                            isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                                            battle.TurnStart();
                                            cycle.SetValue(prev => prev + 1);
                                            break;
                                        
                                        case "SELECT_DONE":
                                            {
                                                if(context.Selection != 0) { context.Error(); return; }
                                                if (battle.TurnEnd()) { _router.Navigate(PageType.REWARD_PAGE); }
                                                
                                                mode.SetValue("WAITING");
                                                // clear
                                                selectedItem.SetValue((ConsumItem?) null);
                                                selectedSKill.SetValue((Skill?) null);
                                                // Battle.PlayerAction = () => { };

                                                // 사이클이 끝난 경우, 죽은 몬스터를 통해 사이클 다시 체크(죽은 몬스터 발생 시 최대 사이클 변화되도록 관리)
                                                if (cycle.GetValue() == GetCurrentMaxTurnCycle()) { RefillTurnCycle(); break; }
                                               
                                                // 진행 중인 경우
                                                isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                                                if (isPlayerTurn.GetValue() == false)
                                                {
                                                    battle.TurnStart();
                                                    cycle.SetValue(prev => prev + 1);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                case false:
                                    {
                                        if(context.Selection != 0) { context.Error(); return; }
                                        if (battle.TurnEnd()) { _router.Navigate(PageType.REWARD_PAGE); }
                                        
                                        if (cycle.GetValue() == GetCurrentMaxTurnCycle()) { RefillTurnCycle(); break; }
                                        
                                        isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                                        if (isPlayerTurn.GetValue() == false || mode.GetValue() == "SELECT_DONE")
                                        {
                                            battle.TurnStart();
                                            cycle.SetValue(prev => prev + 1);
                                        }
                                    }
                                    break;
                            }
                        };
                    })
            },
            {
                PageType.REWARD_PAGE,
                new Renderer((context, states)  =>
                {
                    Player player = ObjectContext.Instance.Player;
                    Battle battle = ObjectContext.Instance.Battle;
                    
                    // rewards
                    var items = battle.RewardItems;
                    var experience = battle.GetTotalExp();
                    var gold = battle.GetTotalGold(); 
                    
                    context.Content = () =>
                    {
                        Console.WriteLine(
                            $"Battle!! - Result\n\n" +
                            $"Victory\n\n" +
                            $"던전에서 몬스터 {battle.MonsterList.Count}마리를 잡았습니다.\n\n" +
                            $"[캐릭터 정보]\n" +
                            $"Lv.1 Chad -> Lv. {player.Level}" +
                            $"Chad\n" +
                            $"HP 100 -> 74\n" +
                            $"exp {player.EXP} -> {player.EXP + experience}\n\n" +
                            $"[획득 아이템]\n" +
                            $"{gold} Gold\n" +
                            $"포션 - 1\n" +
                            $"낡은검 - 1\n\n" +
                            $"0. 다음");
                    };

                    context.Choice = () =>
                    {
                        if(context.Selection != 0) { context.Error(); return; }
                        
                        _router.PopState(3);
                    };
                })
            },
            {
                PageType.SMITHY_PAGE,
                new Renderer((context, states) =>
                {   
                    Player player = ObjectContext.Instance.Player;
                    Smith smith = ObjectContext.Instance.Smith; 
                    smith.SetPlayerEquipItemList();
                    var equipments = player.Inventory.OfType<EquipItem>();

                    var mode = context.States.Get<string>("MODE").Init("VIEW");
                    var result = context.States.Get<ESmithResult>("SMITH_RESULT").Init(ESmithResult.None);

                    context.Content = () =>
                    {
                        Console.WriteLine("강화소\n무기를 강화하실 수 있습니다.\n");
                        for (int i = 0; i < equipments.Count(); i++)
                        {
                            EquipItem item = equipments.ElementAt(i);
                            
                            if(mode.GetValue() == "REINFORCEMENT") Logger.Write($"{i + 1}. ", ConsoleColor.Cyan);
                            Console.WriteLine($"{item.Name} | {item.Explain} | +{item.Stat}");
                        }
                        
                        if(mode.GetValue() == "VIEW") Console.WriteLine($"\n0.나가기\n1. 강화하기");
                        if (mode.GetValue() == "REINFORCEMENT")
                        {
                            Console.WriteLine($"\n0.나가기\n");
                            switch (result.GetValue())
                            {
                                case ESmithResult.Success:
                                    Logger.WriteLine("강화에 성공했습니다.", ConsoleColor.Green);
                                    break;
                                case ESmithResult.Failed_NotEnoughStone:
                                    Logger.WriteLine("강화석이 부족합니다.", ConsoleColor.Red);
                                    break;
                                case ESmithResult.Failed_NotEnoughGold:
                                    Logger.WriteLine("골드가 부족합니다.", ConsoleColor.Red);
                                    break;
                                case ESmithResult.Failed_MaxReinforce:
                                    Logger.WriteLine("최대 강화치입니다.", ConsoleColor.Red);
                                    break;
                            }
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
                                        mode.SetValue("REINFORCEMENT");
                                        break;
                                    default:
                                        context.Error();
                                        break;
                                }
                                break;
                            case "REINFORCEMENT":
                                result.SetValue(ESmithResult.None);
                                
                                if(context.Selection== 0) { mode.SetValue("VIEW"); break; }
                                if(context.Selection > equipments.Count()) { context.Error(); return; }

                                result.SetValue(smith.ReinforceItem(context.Selection));
                                break;
                                
                        }
                        {
                            
                        }
                    };

                })
            }
        };    
    }   
}
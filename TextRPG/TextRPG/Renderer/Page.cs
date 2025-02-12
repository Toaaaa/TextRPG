
using System.Collections;
using System.Diagnostics;
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
    BATTLE_RESULT_PAGE,
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
                        // skip
                        _router.Navigate(PageType.START_PAGE);
                        Console.WriteLine($"스파르타 던전에 오신 여러분 환영합니다.");
                    };

                    // 이름 선택 화면
                    context.Content += () =>
                    {
                        if (mode.GetValue() != "NAME") return;
                        
                        context.SelectionMode = Renderer.SelectionType.text;
                        Console.WriteLine($"원하시는 이름을 설정해주세요.");
                    };
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "NAME") return;
                      
                        player.Name = context.SelectionText;
                        mode.SetValue("CLASS");
                    };

                    // 직업 선택 화면
                    context.Content += () =>
                    {
                        if (mode.GetValue() != "CLASS") return;
                        
                        context.SelectionMode = Renderer.SelectionType.number;
                        Console.WriteLine($"{player.Name}님 원하시는 직업을 선택해주세요.\n\n" + $"1.전사\n2.궁수\n3.도적\n4.마법사");
                    };
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "CLASS") return;
                      
                        if (context.Selection < 0 || context.Selection > classes.Length) { context.Error(); return; }
                        player.Class = classes[context.Selection - 1];
                        _router.Navigate(PageType.START_PAGE);
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
                                $"공격력 : {player.TotalATK - player.AddAttack} [+{player.AddAttack}]\n" +
                                $"방어력 : {player.TotalDEF - player.AddDefense} [+{player.AddDefense}]\n" +
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
                                case 0: _router.PopState(); break;
                                default: context.Error(); break;
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
                    var consumItems = player.Inventory.OfType<ConsumItem>();
                    
                    var mode = states.Get<string>("MODE").Init("VIEW");
                    
                    context.Content = () =>
                    {
                        Console.WriteLine($"인벤토리\n" + $"보유 중인 아이템을 관리할 수 있습니다.\n\n" + $"[장비 목록]");
                        
                        // 목록 표시는 세부 사항으로 인해 유사한 방식이나 하나로 적용하기 어려움.
                        if(!equipments.Any()) Console.WriteLine("보유한 장비가 없습니다.");
                        for (int i = 0; i < equipments.Count(); i++)
                        {
                            EquipItem item = equipments.ElementAt(i);
                            
                            Console.ForegroundColor = item.IsEquip ? ConsoleColor.Blue : ConsoleColor.Gray; 
                            if(item.IsEquip) Console.Write($"[E] ");
                            if(mode.GetValue() == "EQUIPMENT") Console.Write($"{i + 1}. ");
                            Console.WriteLine($"{item.Name} | {item.Explain} | +{item.Stat}");
                        }
                        Console.ResetColor();
                        
                        Console.WriteLine($"\n[아이템 목록]");
                        if(!consumItems.Any()) Console.WriteLine("보유한 아이템이 없습니다.");
                        for (int i = 0; i < consumItems.Count(); i++)
                        {
                            ConsumItem currentItem = consumItems.ElementAt(i);
                            Console.Write($"{i + 1}. ");
                            Console.WriteLine($"{currentItem.Name} | {currentItem.Explain} | {currentItem.Num}개");
                        }

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
                    

                    // 선택 화면일 때
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "VIEW") return;
                        switch (context.Selection)
                        {
                            case 0: _router.PopState(); break;
                            case 1: mode.SetValue("EQUIPMENT"); break;
                            default: context.Error(); break;
                        }
                    };
                    
                    // 장착 모드일 때   
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "EQUIPMENT") return;
                        
                        if (context.Selection == 0) { mode.SetValue("VIEW"); return; }
                        if (context.Selection > equipments.Count()) { context.Error(); return; }
                        
                        // 장착, 미장착
                        EquipItem equipItem = equipments.ElementAt(context.Selection - 1);
                        if (equipItem.IsEquip) player.UnequipItem(equipItem);
                        else player.EquipItem(equipItem);
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

                    // 일반 선택 화면
                    context.Content = () =>
                    {
                        Console.WriteLine($"상점\n" + $"필요한 아이템을 얻을 수 있는 상점입니다.\n\n" + $"[보유 골드]\n{player.Gold} G\n");
                        if(mode.GetValue() == "VIEW") Console.WriteLine("0. 나가기\n1. 구매하기\n2. 판매하기\n\n" + "원하시는 행동을 입력해주세요. >>");
                        if(mode.GetValue() == "CATEGORY") Console.WriteLine("0. 나가기\n1. 장비\n2. 소모품\n\n" + "원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice = () =>
                    {
                        if(mode.GetValue() == "VIEW") switch (context.Selection)
                        {
                            case 0: _router.PopState(); break;
                            case 1: mode.SetValue("CATEGORY"); break;
                            case 2: mode.SetValue("SELLING"); break;
                            default: context.Error(); break;
                        }
                        
                        if(mode.GetValue() == "CATEGORY") switch (context.Selection)
                        {
                            case 0: mode.SetValue("VIEW"); break;
                            case 1: mode.SetValue("BUYING"); category.SetValue("EQUIPMENT"); break;
                            case 2: mode.SetValue("BUYING"); category.SetValue("CONSUM"); break;
                            default: context.Error(); break;
                        }
                    };

                    // 장비 구매일 때
                    context.Content += () =>
                    {
                        if(!(mode.GetValue() == "BUYING" && category.GetValue() == "EQUIPMENT")) return;
                        for (int i = 0; i < equipItems.Count; i++)
                        {
                            Item item = (Item)equipItems[i];
                            bool isExistItem = shop.CheckPlayerHave(item!);
                            Logger.WriteLine($"{i + 1}. { item.Name} | {item.Explain} | {(isExistItem? "구매 완료" : item.Price +"G")}", isExistItem ? ConsoleColor.DarkCyan: ConsoleColor.Gray);
                        }
                                        
                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("\n구매를 성공했습니다.", ConsoleColor.Green);
                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("\n이미 구입한 상품입니다.", ConsoleColor.Red);
                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("\n골드가 부족합니다.", ConsoleColor.Red);
                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice += () =>
                    {
                        if(!(mode.GetValue() == "BUYING" && category.GetValue() == "EQUIPMENT")) return;
                        if (context.Selection == 0) { mode.SetValue("CATEGORY"); category.SetValue("NONE"); result.SetValue(TradeResult.None); return; }
                        if (context.Selection > equipItems.Count) { context.Error(); return; }
                        
                        result.SetValue(shop.BuyEquipItem(context.Selection));
                    };
                    
                    // 아이템 구매일 때
                    context.Content += () =>
                    {
                        if(!(mode.GetValue() == "BUYING" && category.GetValue() == "CONSUM")) return;
                        for (int i = 0; i < consumItems.Count; i++)
                        {
                            ConsumItem currentItem = (ConsumItem)consumItems[i];
                            var existItemByName = player.Inventory.Find(item => item.Name == currentItem.Name) as ConsumItem; // 다른 객체라 값으로 비교
                            Console.WriteLine($"{i + 1}. { currentItem.Name} | {currentItem.Explain} | {currentItem.Price}G | {(existItemByName == null ? "0" :  existItemByName.Num)} 개 보유 중");
                        }
                                     
                        if(result.GetValue() == TradeResult.Success) Logger.WriteLine("\n구매를 성공했습니다.", ConsoleColor.Green);
                        if(result.GetValue() == TradeResult.Failed_AlreadyHave) Logger.WriteLine("\n이미 구입한 상품입니다.", ConsoleColor.Red);
                        if(result.GetValue() == TradeResult.Failed_NotEnoughGold) Logger.WriteLine("\n골드가 부족합니다.", ConsoleColor.Red);
                        Console.WriteLine("\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice += () =>
                    {
                        if(!(mode.GetValue() == "BUYING" && category.GetValue() == "CONSUM")) return;
                        if (context.Selection == 0) { mode.SetValue("CATEGORY"); category.SetValue("NONE"); result.SetValue(TradeResult.None); return; }
                        if (context.Selection > consumItems.Count) { context.Error(); return; }
                        
                        result.SetValue(shop.BuyConsumItem(context.Selection));
                    };

                    // 판매 일 때
                    context.Content += () =>
                    {
                        if(mode.GetValue() != "SELLING") return;
                        
                        for (int i = 0; i < inventory.Count; i++)
                        {
                            Item item = inventory[i];
                            Console.WriteLine($"{i + 1}. {item.Name} | {item.Explain} | {Math.Round(item.Price * 0.85)}G");
                        }
                                    
                        Console.WriteLine("\n0. 나가기\n\n" + "원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice += () =>
                    {
                        if(mode.GetValue() != "SELLING") return;

                        if (context.Selection == 0) { mode.SetValue("VIEW"); result.SetValue(TradeResult.None); return; }
                        if (context.Selection > inventory.Count) { context.Error(); return; }
                                
                        shop.SellItem(context.Selection - 1);
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
                        
                        // 배틀 관련 : null 방지 추가
                        int GetCurrentMaxTurnCycle() => (battle.GetAliveMonsterList() ?? []).Count() + 1;
                        void RefillTurnCycle() {
                            // 현재 남은 몬스터를 기준으로 queue 재할당
                            List<Actor> currentActors = new List<Actor>() {};
                            currentActors.AddRange(battle.GetAliveMonsterList() ?? []);
                            currentActors.Add(player);
                            battle.TurnQueue = battle.GetTurnQueue(currentActors);
                                                
                            // 만일 사이클이 끝난 뒤 다시 선택 화면을 우선 처리하고 싶다면 분리 종료
                            isPlayerTurn.SetValue(true);
                            cycle.SetValue(0);
                        }
                        void ExecuteTurnBySelectDone()
                        {
                            // 다음 턴 진행
                            mode.SetValue("SELECT_DONE");
                            isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                            battle.TurnStart();
                            cycle.SetValue(prev => prev + 1);
                        }
                        
                        var consumItems = player.Inventory.OfType<ConsumItem>();
                        var selectedSKill = states.Get<Skill?>("SELECTED_SKILL").Init(null);
                        var selectedItem = states.Get<ConsumItem?>("SELECTED_ITEM").Init(null);
                        // do: 한번만 호출 필요(스킬 클래스에 스태틱으로)
                        Dictionary<string, Skill> skills = Skill.LoadSkillDictionary(Path.Combine(Path.GetFullPath(@"../../../Objects/SkillList.json")));
                        dynamic previousPlayerInfo = null!; context.Mount = () => { previousPlayerInfo =  new { Level = player.Level, HP = player.HP, EXP = player.EXP }; };
                        
                        // [선택 화면]
                        context.Content += () =>
                        {
                            if (isPlayerTurn.GetValue() == false || mode.GetValue() == "SELECT_DONE") return;

                            Logger.WriteLine($"Battle!!\n", ConsoleColor.Yellow);

                            // show monster list
                            for (int index = 0; index < dungeon.MonsterList.Count; index++)
                            {
                                Monster monster = dungeon.MonsterList[index];
                                if (mode.GetValue() == "CHOOSE_TARGET")
                                    Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                if (battle.Target != null && battle.Target.Contains(monster))
                                    Console.ForegroundColor = ConsoleColor.Cyan;

                                if (monster.IsDead) Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write($"Lv.{monster.Level} {monster.Name} - ");
                                Console.WriteLine(monster.IsDead ? "Dead" : $"HP {monster.HP}");

                                Console.ResetColor();
                            }

                            // show player info
                            Console.WriteLine($"\n[내정보]\n" + $"Lv.{player.Level}  Chad ({player.Class}) \n" + $"HP {player.HP}/{player.MaxHP}\n");
                            
                            if(mode.GetValue() == "WAITING") Console.WriteLine("0. 나가기\n1. 공격\n2. 스킬\n3. 아이템");
                            if(mode.GetValue() == "CHOOSE_TARGET") Console.WriteLine("0. 취소");
                        };
                        // [기본 선택]
                        context.Choice += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "WAITING")) return;
                            
                            string[] modes = ["CHOOSE_TARGET", "SELECT_SKILL", "USING_ITEM"];

                            if(context.Selection == 0) { _router.PopState();; return; }
                            if(context.Selection > modes.Length) { context.Error(); return; }

                            mode.SetValue(modes[context.Selection - 1]);
                        };
                        // [대상 선택]
                        context.Choice += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "CHOOSE_TARGET")) return;

                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }
                            if (context.Selection > dungeon.MonsterList.Count) { context.Error(); return; }
                            if ((bool)battle.GetMonsterIsDead(context.Selection - 1)) { context.Error(); return; } // 죽은 몬스터를 선택한 경우
                            
                            Monster selectedMonster = battle.MonsterList[context.Selection - 1];

                            // fix : 다중 공격 - nullable 체크 
                            if (selectedSKill.GetValue()?.Name == "이단 배기")
                            {
                                if (battle.Target == null) { battle.Target = new List<Actor>(); }
                                if (battle.Target.Contains(selectedMonster)) { context.Error("이미 선택한 대상입니다."); return; }
                                battle.Target.Add(battle.MonsterList[context.Selection - 1]);
                                // 스킬은 2번 선택이나, 대상이 1명 남았을 경우 체크
                                if (battle.GetAliveMonsterList().Count() >= 2 && battle.Target.Count < 2) { return; }
                            }
                            // 단일 공격// 대상 지정, 플레이어 행동 결정 완료
                            else { battle.SetTargetMonster([selectedMonster]); }
                            // 일반 공격
                            if (selectedSKill.GetValue() == null) { Battle.PlayerAction = () => battle.Target.ForEach(target => battle.PlayerAttack((target as Monster)!)); }
                            // 스킬 공격 // 사용된 마나 감소시키기
                            else { Battle.PlayerAction = () => { battle.Target.ForEach(target => battle.PlayerSkillAttack(target as Monster, selectedSKill.GetValue())); player.MP -= selectedSKill.GetValue().Mana; }; }

                            ExecuteTurnBySelectDone();
                        };


                        // [스킬 선택 화면]
                        context.Content += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "SELECT_SKILL")) return;
                            
                            for (int index = 0; index < skills.Count(); index++)
                            {
                                Skill currentSkill = skills.ElementAt(index).Value; 
                                Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                Console.WriteLine($"{currentSkill.Name} | {currentSkill.Explain} | {currentSkill.Mana}MP");
                            }
                            Console.WriteLine("\n0. 취소");
                        };
                        context.Choice += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "SELECT_SKILL")) return;
                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }                                           
                            if (context.Selection > skills.Count()) { context.Error(); return; }
                                            
                            Skill currentSkill = skills.ElementAt(context.Selection - 1).Value;
                            if(currentSkill.Mana > player.MP) { context.Error("마나가 부족합니다."); return; }
                            selectedSKill.SetValue(currentSkill);
                                            
                            // 전체 공격일 경우, 선택을 생략한다.
                            if (currentSkill.MultiHit)
                            {
                                // 타겟 선정 페이지로 갈 필요가 없어서 액션도 여기서 설정해줘야 함.
                                battle.SetTargetMonster(battle.GetAliveMonsterList());
                                Battle.PlayerAction = () =>
                                {
                                    battle.Target.ForEach(target => battle.PlayerSkillAttack(target as Monster, currentSkill));
                                    player.MP -= currentSkill.Mana;
                                };

                                ExecuteTurnBySelectDone();
                                return;
                            }
                            mode.SetValue("CHOOSE_TARGET");
                        };
                        
                        // [아이템 선택 화면]
                        context.Content += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "USING_ITEM")) return;
                            IEnumerable<ConsumItem> items = player.Inventory.OfType<ConsumItem>();
                            for (int index = 0; index < items.Count(); index++)
                            {
                                ConsumItem item = items.ElementAt(index);
                                Logger.Write($"{index + 1} ", ConsoleColor.Cyan);
                                // 아이템 갯수 확인 필요
                                Console.WriteLine($"{item.Name} | {item.Explain}");
                            }
                            Console.WriteLine("\n0. 취소");
                        };
                        context.Choice += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "USING_ITEM")) return;
                            if(context.Selection == 0) { mode.SetValue("WAITING"); return; }
                            if (context.Selection > consumItems.Count() + 1) { context.Error(); return; }
                                            
                            selectedItem.SetValue(consumItems.ElementAt(context.Selection - 1));
                            Battle.PlayerAction = () =>
                            {
                                selectedItem.GetValue().UseItem(player, EConsumItem.Potion);
                                // 소모된 아이템 제거
                                player.Inventory.Remove(selectedItem.GetValue());
                            };
                                            
                            ExecuteTurnBySelectDone();
                        };
                        
                        
                        // [플레이어의 턴]
                        context.Content += () =>
                        {
                            if(!(isPlayerTurn.GetValue() == true && mode.GetValue() == "SELECT_DONE")) return;
                            
                            // 아이템 사용인 경우
                            if (selectedItem.GetValue() != null)
                            {
                                Console.WriteLine($"{player.Name} 가 {selectedItem.GetValue().Name}을 사용했습니다.\n" 
                                    + $"HP {battle.LastHp} -> {player.HP}\n\n" + $"HP {battle.LastMp} -> {player.MP}\n\n"
                                    + $"0. 다음\n\n원하시는 행동을 입력해주세요. >>");
                                return;
                            }

                            //치명타 체크                            
                            if(player.IsCritical()) Console.WriteLine("[치명타 공격 발생!]");
                            // 공격 방식 알림
                            if (selectedSKill.GetValue() != null) Console.WriteLine($"{player.Name}가 {selectedSKill.GetValue().Name} 스킬을 사용했습니다.\n");
                            else Console.WriteLine($"{player.Name}의 일반 공격!\n");
                            
                            foreach (Monster monster in battle.Target)
                            {
                                Console.WriteLine(
                                    $"Lv.{monster.Level} {monster.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n" + 
                                    $"HP {monster.BeforeHP} -> {monster.HP}\n");
                            }
                            
                            Console.WriteLine($"0. 다음\n\n원하시는 행동을 입력해주세요. >>");
                        };
                        context.Choice += () =>
                        {
                            if (!(isPlayerTurn.GetValue() == true && mode.GetValue() == "SELECT_DONE")) return;
                            if(context.Selection != 0) { context.Error(); return; }

                            if (battle.CheckBattleEnd()) { _router.Navigate(PageType.BATTLE_RESULT_PAGE, new { previousPlayerInfo }); }
                            
                            // clear
                            mode.SetValue("WAITING");
                            selectedItem.SetValue((ConsumItem?) null);
                            selectedSKill.SetValue((Skill?) null);
                            battle.Target.Clear();
                            // Battle.PlayerAction = () => { };

                            // 사이클이 끝난 경우, 죽은 몬스터를 통해 사이클 다시 체크(죽은 몬스터 발생 시 최대 사이클 변화되도록 관리)
                            if (cycle.GetValue() == GetCurrentMaxTurnCycle()) { RefillTurnCycle(); return; }
                                               
                            // 진행 중인 경우 계속 진행
                            isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                            // fix: 상태 저장 방식의 변경으로 인해 상태값으로 바로 인식 불가능 
                            if (battle.GetIsPlayerTurn() == false)
                            {
                                battle.TurnStart();
                                cycle.SetValue(prev => prev + 1);
                            }
                        };
                            
                        
                        // [적군의 턴]
                        context.Content += () =>
                        {
                            if(isPlayerTurn.GetValue() == true) return;
                            
                            Actor monster = battle.CurrentActor;
                            
                            //치명타 체크                            
                            if(monster.IsCritical()) Console.WriteLine("[치명타 공격 발생!]\n");
                            Console.WriteLine(
                                $"{monster.Name} 의 공격!\n" + $"{player.Name} 을(를) 맞췄습니다. [데미지 : {battle.LastDamage}]\n\n" +
                                $"Lv.{player.Level} {player.Name}\nHP {player.HP + battle.LastDamage} -> {player.HP}\n");
                            
                            Console.WriteLine($"0. 다음\n\n원하시는 행동을 입력해주세요. >>");
                        };
                        context.Choice += () =>
                        {
                            if(isPlayerTurn.GetValue() == true) return;
                            if(context.Selection != 0) { context.Error(); return; }
                            if(battle.CheckBattleEnd() || player.HP <= 0) { _router.Navigate(PageType.BATTLE_RESULT_PAGE, new { previousPlayerInfo }); }
                            if (cycle.GetValue() == GetCurrentMaxTurnCycle()) { RefillTurnCycle(); return; }
                                        
                            // 다음 턴도 몬스터일 경우 다음 턴 진행
                            isPlayerTurn.SetValue(battle.GetIsPlayerTurn());
                            if (battle.GetIsPlayerTurn() == false || mode.GetValue() == "SELECT_DONE")
                            {
                                battle.TurnStart();
                                cycle.SetValue(prev => prev + 1);
                            }
                        };
                    })
            },
            {
                PageType.BATTLE_RESULT_PAGE,
                new Renderer((context, states)  =>
                {
                    Player player = ObjectContext.Instance.Player;
                    Battle battle = ObjectContext.Instance.Battle;
                    
                    bool isVictory = player.HP > 0;
                    // 각 페이지들에서 전달하는 정보가 다를 경우를 대비하여 key-value 로 관리 필요.
                    dynamic? parameters = null; context.Mount += () => parameters = context.States.GetParams<object>();

                    // [승리화면] - 보상 등록
                    context.Mount += () =>
                    {
                        if (!isVictory) return;
                        battle.TurnEnd();
                        battle.EndBattle();
                    };
                    // [승리 화면]
                    context.Content = () =>
                    {
                        if (!isVictory) return;
                        dynamic previousPlayerInfo = parameters.previousPlayerInfo;
                        
                        // 레벨업 정보, 처음 체력 정보 알기 필요
                        Console.WriteLine(
                            $"Battle!! - Result\n\n" + $"Victory\n\n" +
                            $"던전에서 몬스터 {battle.MonsterList.Count}마리를 잡았습니다.\n\n" +
                            $"[캐릭터 정보]\n" +
                            $"Lv.{previousPlayerInfo.Level} -> Lv. {player.Level}\n" +
                            $"HP {previousPlayerInfo.HP} -> {player.HP}\n" +
                            $"EXP {previousPlayerInfo.EXP} -> {player.EXP}\n" +
                            $"[획득 아이템]\n" +
                            $"{battle.GetTotalGold()} Gold\n");
                        
                        battle.RewardItems?.ForEach(Console.WriteLine);
                        
                        Console.WriteLine($"0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    
                    // [패배 화면]
                    context.Content += () =>
                    {
                        if (isVictory) return;
                        Console.WriteLine("[게임 오버]\n패배했습니다.\n\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    
                    // [공통 선택]
                    context.Choice += () =>
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
                    Smith smith = ObjectContext.Instance.Smith; 
                    var equipments = smith.PlayerEquipItemList;
                    
                    var mode = context.States.Get<string>("MODE").Init("VIEW");
                    var result = context.States.Get<ESmithResult>("SMITH_RESULT").Init(ESmithResult.None);

                    // 인벤토리 목록과 일치되도록 다시 체크
                    context.Mount = () => smith.SetPlayerEquipItemList();
                    
                    // [공통 화면]
                    context.Content = () =>
                    {
                        Console.WriteLine("강화소\n무기를 강화하실 수 있습니다.\n\n[장비 목록]\n");
                        if(equipments.Count == 0) Console.WriteLine("강화할 무기가 없습니다.");
                        for (int i = 0; i < equipments.Count(); i++)
                        {
                            EquipItem item = equipments.ElementAt(i);
                            
                            if(mode.GetValue() == "REINFORCEMENT") Logger.Write($"{i + 1}. ", ConsoleColor.Cyan);
                            Console.WriteLine($"{item.Name} | {item.Explain} | +{item.Stat}");
                        }
                    };

                    // [VIEW 화면]
                    context.Content += () =>
                    {
                        if (mode.GetValue() != "VIEW") return;
                        Console.WriteLine($"\n0. 나가기\n1. 강화하기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "VIEW") return;
                        switch (context.Selection)
                        {
                            case 0: _router.PopState(); break;
                            case 1: mode.SetValue("REINFORCEMENT"); break;
                            default: context.Error(); break;
                        }
                    };

                    // [강화 페이지]
                    context.Content += () =>
                    {
                        if (mode.GetValue() != "REINFORCEMENT") return;
                        switch (result.GetValue())
                        {
                            case ESmithResult.None: break;
                            case ESmithResult.Success: Logger.WriteLine("강화에 성공했습니다.", ConsoleColor.Green); break;
                            case ESmithResult.Failed_NotEnoughStone: Logger.WriteLine("강화석이 부족합니다.", ConsoleColor.Red); break;
                            case ESmithResult.Failed_NotEnoughGold: Logger.WriteLine("골드가 부족합니다.", ConsoleColor.Red); break;
                            case ESmithResult.Failed_MaxReinforce: Logger.WriteLine("최대 강화치입니다.", ConsoleColor.Red); break;
                        }
                        Console.WriteLine($"\n0. 나가기\n\n원하시는 행동을 입력해주세요. >>");
                    };
                    context.Choice += () =>
                    {
                        if (mode.GetValue() != "REINFORCEMENT") return;
                        result.SetValue(ESmithResult.None);
                                
                        if(context.Selection == 0) { mode.SetValue("VIEW"); return; }
                        if(context.Selection > equipments.Count()) { context.Error(); return; }

                        result.SetValue(smith.ReinforceItem(context.Selection));
                    };
                })
            }
        };    
    }   
}
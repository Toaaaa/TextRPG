using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Objects.Items;

public enum ESmithResult
{
    None,
    Success,
    Failed_MaxReinforce,
    Failed_NotEnoughStone,
    Failed_NotEnoughGold
}

namespace TextRPG.Objects.Smith
{
    public class Smith
    {
        public Smith()
        {
            PlayerEquipItemList = new List<EquipItem>();
            ReinforceRequirement = new List<ReinforceRequirementData>();
        }

        public List<EquipItem> PlayerEquipItemList { get; private set; }

        public List<ReinforceRequirementData> ReinforceRequirement { get; private set; } // 강화에 필요한 재료들을 모아놓은 컬렉션

        public class ReinforceRequirementData
        {
            public int ReinforcePrice; // 강화에 필요한 가격
            public int RequirementReinforceStone; // 강화에 필요한 강화석
        }

        public void SetPlayerEquipItemList()
        {
            PlayerEquipItemList.Clear();
            ReinforceRequirement.Clear();

            for (int i = 0; i < ObjectContext.Instance.Player.Inventory.Count; i++)
            {
                if (ObjectContext.Instance.Player.Inventory[i] is EquipItem)
                {
                    PlayerEquipItemList.Add((EquipItem)ObjectContext.Instance.Player.Inventory[i]);
                }
            }

            for (int i = 0; i < PlayerEquipItemList.Count; i++)
            {
                ReinforceRequirement.Add(new ReinforceRequirementData());
                ReinforceRequirement[i].ReinforcePrice = (PlayerEquipItemList[i].ReinforcementLevel * 300) + 500;
                ReinforceRequirement[i].RequirementReinforceStone = PlayerEquipItemList[i].ReinforcementLevel + 1;
            }
        }

        public ESmithResult ReinforceItem(int _Choice)
        {
            SetPlayerEquipItemList();

            EquipItem reinforceItem = PlayerEquipItemList[_Choice - 1];
            int reinforcePrice = ReinforceRequirement[_Choice - 1].ReinforcePrice;
            int requirementReinforceStone = ReinforceRequirement[_Choice - 1].RequirementReinforceStone;

            if (null == reinforceItem) //null 체크
            {
                Logger.Debug("존재하지 않는 장비를 강화하려했습니다.");
            }

            if (10 > reinforceItem.ReinforcementLevel) // 최대 강화 체크
            {
                ConsumItem reinforceStone = ObjectContext.Instance.Player.Inventory.FirstOrDefault(item => item.Name == "강화석") as ConsumItem;
                if (null == reinforceStone) // 강화석 유무 체크
                {
                    return ESmithResult.Failed_NotEnoughStone;
                }

                if (reinforceStone.Num >= requirementReinforceStone) // 강화에 충분한 양의 강화석이 있는지 체크
                {
                    reinforcePrice = (reinforceItem.ReinforcementLevel * 300) + 500;

                    if (reinforcePrice <= ObjectContext.Instance.Player.Gold) // 강화비용이 충분한지 체크
                    {
                        EquipItem shopItem = (EquipItem)ObjectContext.Instance.Shop.EquipItemShop[reinforceItem.Name];

                        ObjectContext.Instance.Player.Gold -= reinforcePrice;
                        reinforceStone.Num -= requirementReinforceStone;

                        reinforceItem.ReinforcementLevel++;
                        reinforceItem.ReinforceStat = shopItem.Stat + ((int)Math.Ceiling(shopItem.Stat * 0.25) * reinforceItem.ReinforcementLevel);
                        reinforceItem.TotalStat = reinforceItem.Stat + reinforceItem.ReinforceStat;

                        if (0 == reinforceStone.Num) // 이번 강화로 강화석이 0이 되었는지 체크
                        {
                            ObjectContext.Instance.Player.RemoveItem(reinforceStone);
                        }

                        return ESmithResult.Success;
                    }
                    else
                    {
                        return ESmithResult.Failed_NotEnoughGold;
                    }
                }
                else
                {
                    return ESmithResult.Failed_NotEnoughStone;
                }
            }
            else
            {
                return ESmithResult.Failed_MaxReinforce;
            }
        }
    }
}

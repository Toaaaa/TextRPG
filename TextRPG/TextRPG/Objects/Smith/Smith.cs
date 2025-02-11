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
        }

        public List<EquipItem> PlayerEquipItemList { get; private set; }

        public void SetPlayerEquipItemList()
        {
            PlayerEquipItemList.Clear();
            for (int i = 0; i < ObjectContext.Instance.Player.Inventory.Count; i++) 
            {
                if (ObjectContext.Instance.Player.Inventory[i] is EquipItem)
                {
                    PlayerEquipItemList.Add((EquipItem)ObjectContext.Instance.Player.Inventory[i]);
                }
            }
        }

        public ESmithResult ReinforceItem(int _Choice)
        {
            EquipItem reinforceItem = PlayerEquipItemList[_Choice - 1];
            if (10 > reinforceItem.ReinforcementLevel)
            {
                ConsumItem reinforceStone = (ConsumItem)ObjectContext.Instance.Player.Inventory.FirstOrDefault(item => item.Name == "강화석");
                if (reinforceStone.Num > 0)
                {
                    int reinforcePrice = (reinforceItem.ReinforcementLevel * 300) + 500;

                    if (reinforcePrice <= ObjectContext.Instance.Player.Gold)
                    {
                        EquipItem shopItem = (EquipItem)ObjectContext.Instance.Shop.EquipItemShop[reinforceItem.Name];

                        ObjectContext.Instance.Player.Gold -= reinforcePrice;
                        reinforceItem.ReinforcementLevel++;
                        reinforceItem.Stat = shopItem.Stat + ((int)Math.Ceiling(shopItem.Stat * 0.25) * reinforceItem.ReinforcementLevel);

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

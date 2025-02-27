using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Specialized;
using TextRPG.Objects.Items;
using TextRPG.Objects;
using System.Collections;
using System.IO;
using System.Text.Json.Serialization;

public enum TradeResult
{
    None,
    Success,
    Failed_AlreadyHave,
    Failed_NotEnoughGold
}

namespace TextRPG.Objects.Shop
{
    public class Shop
    {
        public Shop()
        {
            AllItem = new Dictionary<string, Item>();

            EquipItemShop = new OrderedDictionary();
            ConsumItemShop = new OrderedDictionary();
            LoadItem();
        }

        //public
        //모든 아이템 보관하는 Dictionary
        public Dictionary<string, Item> AllItem;

        public OrderedDictionary EquipItemShop { get; private set; }
        public OrderedDictionary ConsumItemShop { get; private set; }

        //public

        //플레이어 소유여부 확인
        public bool CheckPlayerHave(Item _CheckItem)
        {
            return ObjectContext.Instance.Player.Inventory.Any(item => item.Name == _CheckItem.Name);
        }

        //장비 구매
        public TradeResult BuyEquipItem(int _Choice)
        {
            EquipItem equipItem = (EquipItem)EquipItemShop[_Choice - 1];
            if (equipItem != null)
            {
                if (CheckPlayerHave(equipItem))
                {
                    return TradeResult.Failed_AlreadyHave;
                }
                else
                {
                    if (ObjectContext.Instance.Player.Gold >= equipItem.Price)
                    {
                        ObjectContext.Instance.Player.GetItem(new EquipItem(equipItem));
                        ObjectContext.Instance.Player.Gold -= equipItem.Price;

                        return TradeResult.Success;
                    }
                    else
                    {
                        return TradeResult.Failed_NotEnoughGold;
                    }
                }
            }
            else
            {
                Logger.Debug("존재하지 않는 장비 아이템을 선택했습니다.");
                return TradeResult.None;
            }
        }

        public void AddEquipItem(string _ItemName)
        {
            EquipItem equipItem = EquipItemShop[_ItemName] as EquipItem;

            if (!CheckPlayerHave(equipItem))
            {
                ObjectContext.Instance.Player.GetItem(new EquipItem(equipItem));
            }
        }

        //소모품 구매
        public TradeResult BuyConsumItem(int _Choice)
        {
            ConsumItem consumItem = (ConsumItem)ConsumItemShop[_Choice - 1];
            if (consumItem != null)
            {
                if (ObjectContext.Instance.Player.Gold >= consumItem.Price)
                {
                    AddConsumItem(consumItem.Name);
                    ObjectContext.Instance.Player.Gold -= consumItem.Price;

                    return TradeResult.Success;
                }
                else
                {
                    return TradeResult.Failed_NotEnoughGold;
                }
            }
            else
            {
                Logger.Debug("존재하지 않는 소모 아이템을 선택했습니다.");
                return TradeResult.None;
            }
        }

        public void AddConsumItem(string _ItemName, int _AddNum = 1)
        {
            ConsumItem consumItem = ConsumItemShop[_ItemName] as ConsumItem;
            if (CheckPlayerHave(consumItem))
            {
                ConsumItem cItem = ObjectContext.Instance.Player.Inventory.OfType<ConsumItem>().FirstOrDefault(item => item.Name == consumItem.Name);
                cItem.Num += _AddNum;
            }
            else
            {
                ConsumItem cItem = new ConsumItem(consumItem);
                ObjectContext.Instance.Player.GetItem(cItem);
                cItem.Num = _AddNum;
            }
        }

        //아이템 판매
        public void SellItem(int _Choice)
        {
            List<Item> playerInven = ObjectContext.Instance.Player.Inventory;

            if (null != playerInven[_Choice])
            {
                if (playerInven[_Choice] is EquipItem equipItem)
                {
                    ObjectContext.Instance.Player.Gold += (int)(equipItem.Price * 0.85);
                    ObjectContext.Instance.Player.RemoveItem(equipItem);
                }
                else if (playerInven[_Choice] is ConsumItem consumItem)
                {
                    if (1 == consumItem.Num)
                    {
                        ObjectContext.Instance.Player.Gold += (int)(consumItem.Price * 0.85);
                        ObjectContext.Instance.Player.RemoveItem(consumItem);
                    }
                    else if (0 < consumItem.Num)
                    {
                        ObjectContext.Instance.Player.Gold += (int)(consumItem.Price * 0.85);
                        consumItem.Num -= 1;
                    }
                }
            }
            else
            {
                Logger.Debug("인벤토리에 존재하지 않는 아이템을 판매하려 했습니다.");
            }
        }

        //private
        void LoadItem()
        {
            string filePath;
            string json;

            // 장비 아이템 읽어오기
            LoadItemFile<EquipItem, OrderedDictionary>(@"../../../Objects/Items/EquipItem.json", EquipItemShop);

            //소모품 읽어오기
            LoadItemFile<ConsumItem, OrderedDictionary>(@"../../../Objects/Items/ConsumItem.json", ConsumItemShop);
        }

        void LoadItemFile<TClass, TDictionary>(string _FilePath, TDictionary _Dictionary)
            where TClass : Item
            where TDictionary : IDictionary
        {
            string json;

            _FilePath = Path.Combine(Path.GetFullPath(_FilePath));
            if (!File.Exists(_FilePath))
            {
                Logger.Debug("파일을 찾을 수 없습니다.");
                return;
            }

            json = File.ReadAllText(_FilePath);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            Dictionary<string, TClass>? itemData = JsonSerializer.Deserialize<Dictionary<string, TClass>>(json, options);

            foreach (var data in itemData)
            {
                string key = data.Key;
                TClass item = data.Value;

                AllItem[key] = item;

                _Dictionary[key] = item;
            }
        }
    }
}

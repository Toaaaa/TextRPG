﻿using System;
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

        public OrderedDictionary EquipItemShop { get; }
        public OrderedDictionary ConsumItemShop { get; }

        //public

        //플레이어 소유여부 확인
        public bool CheckPlayerHave(Item _CheckItem)
        {
            return ObjectContext.Instance.Player.Inventory.Any(item => item.Name == _CheckItem.Name);
        }

        //장비 구매
        public string BuyEquipItem(int _Choice)
        {
            EquipItem equipItem = (EquipItem)EquipItemShop[_Choice - 1];
            if (equipItem != null)
            {
                if (CheckPlayerHave(equipItem))
                {
                    return "이미 구매한 장비입니다.";
                }
                else
                {
                    if (ObjectContext.Instance.Player.Gold > equipItem.Price)
                    {
                        ObjectContext.Instance.Player.GetItem(equipItem);
                        ObjectContext.Instance.Player.Gold -= equipItem.Price;

                        return $"{equipItem.Name}을/를 1개 구매하였습니다.";
                    }
                    else
                    {
                        return "금액이 부족합니다.";
                    }
                }
            }
            else
            {
                return "존재하지 않는 아이템입니다.";
            }
        }

        //소모품 구매
        public string BuyConsumItem(int _Choice)
        {
            ConsumItem consumItem = (ConsumItem)ConsumItemShop[_Choice - 1];
            if (consumItem != null)
            {
                if (CheckPlayerHave(consumItem))
                {
                    if (ObjectContext.Instance.Player.Gold > consumItem.Price)
                    {
                        ConsumItem CItem = ObjectContext.Instance.Player.Inventory.FirstOrDefault(consumItem) as ConsumItem;
                        CItem.Num += 1;
                        ObjectContext.Instance.Player.Gold -= consumItem.Price;

                        return $"{consumItem.Name}을/를 1개 구매하였습니다.";
                    }
                    else
                    {
                        return "금액이 부족합니다.";
                    }
                }
                else
                {
                    if (ObjectContext.Instance.Player.Gold > consumItem.Price)
                    {
                        ObjectContext.Instance.Player.GetItem(consumItem);
                        ObjectContext.Instance.Player.Gold -= consumItem.Price;

                        return $"{consumItem.Name}을/를 1개 구매하였습니다.";
                    }
                    else
                    {
                        return "금액이 부족합니다.";
                    }
                }
            }
            else
            {
                return "존재하지 않는 아이템입니다.";
            }
        }

        //아이템 판매
        public void SellItem(int _Choice)
        {
            List<Item> PlayerInven = ObjectContext.Instance.Player.Inventory;
            ObjectContext.Instance.Player.RemoveItem(PlayerInven[_Choice]);
        }

        //private
        private void LoadItem()
        {
            string filePath;
            string json;

            // 장비 아이템 읽어오기
            {
                filePath = Path.GetFullPath(@"..\..\..\Objects\Items\EquipItem.json");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("파일을 찾을 수 없습니다.");
                    return;
                }

                json = File.ReadAllText(filePath);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                Dictionary<string, EquipItem> itemData = JsonSerializer.Deserialize<Dictionary<string, EquipItem>>(json, options);

                foreach (var data in itemData)
                {
                    string key = data.Key;
                    EquipItem item = data.Value;

                    AllItem[key] = item;

                    EquipItemShop[key] = item;
                }
            }

            //소모품 읽어오기
            {
                filePath = Path.GetFullPath(@"..\..\..\Objects\Items\ConsumItem.json");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("파일을 찾을 수 없습니다.");
                    return;
                }

                json = File.ReadAllText(filePath);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                Dictionary<string, ConsumItem> itemData = JsonSerializer.Deserialize<Dictionary<string, ConsumItem>>(json, options);

                foreach (var data in itemData)
                {
                    string key = data.Key;
                    ConsumItem item = data.Value;

                    AllItem[key] = item;

                    ConsumItemShop[key] = item;
                }
            }
        }
    }
}

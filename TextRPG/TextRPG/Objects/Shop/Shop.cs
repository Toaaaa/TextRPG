using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Specialized;
using TextRPG.Objects.Items;
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

        public Dictionary<string, Item> AllItem;

        public OrderedDictionary EquipItemShop;
        public OrderedDictionary ConsumItemShop;

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

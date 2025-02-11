using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EEquipPart
{
    None,
    Weapon,
    Armor,
}

namespace TextRPG.Objects.Items
{
    public class EquipItem : Item
    {
        // 생성자
        public EquipItem()
        {
            ReinforceStat = 0;

            ReinforcementLevel = 0;

            IsEquip = false;
        }

        public EquipItem(string _Name, int _Stat, EEquipPart _Part)
        {
            Name = _Name;

            Part = _Part;

            Stat = _Stat;
            TotalStat = _Stat;
            ReinforceStat = 0;

            ReinforcementLevel = 0;

            IsEquip = false;
        }

        public EquipItem(EquipItem _Origin)
        {
            Name = _Origin.Name;
            Explain = _Origin.Explain;
            Price = _Origin.Price;

            Part = _Origin.Part;

            Stat = _Origin.Stat;
            TotalStat = Stat;
            ReinforceStat = 0;

            ReinforcementLevel = _Origin.ReinforcementLevel;
            IsEquip = _Origin.IsEquip;
        }

        //public
        public EEquipPart Part { get; set; } // 장착 부위

        public int TotalStat { get; set; } // 총 스탯
        public int Stat { get; set; } // 기초 스탯
        public int ReinforceStat { get; set; } // 강화 스탯

        public bool IsEquip { get; set; } // 장착 유무

        public int ReinforcementLevel { get; set; } // 강화 단계
    }
}

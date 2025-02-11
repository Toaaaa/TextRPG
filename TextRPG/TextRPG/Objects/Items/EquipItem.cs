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
        public EquipItem()
        {
            ReinforcementLevel = 0;

            IsEquip = false;
        }

        public EquipItem(string _Name, int _Stat, EEquipPart _Part)
        {
            Name = _Name;

            Part = _Part;
            Stat = _Stat;

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

            ReinforcementLevel= _Origin.ReinforcementLevel;
            IsEquip = _Origin.IsEquip;
        }

        //public
        public EEquipPart Part { get; set; }

        public int Stat { get; set; }

        public bool IsEquip { get; set; }

        public int ReinforcementLevel { get; set; }
    }
}

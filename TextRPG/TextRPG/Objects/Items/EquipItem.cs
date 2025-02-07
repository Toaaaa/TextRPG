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
        }

        public EquipItem(string _Name, int _Stat, EEquipPart _Part)
        {
            Name = _Name;
            Part = _Part;

            Stat = _Stat;  
            IsEquip = false;
        }

        //public
        public EEquipPart Part { get; set; }

        public int Stat { get; set; }

        public bool IsEquip { get; set; }
    }
}

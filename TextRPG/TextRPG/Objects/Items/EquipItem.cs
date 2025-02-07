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

        public EquipItem(string _Name, int _Status, EEquipPart _Part)
        {
            Name = _Name;
            Part = _Part;

            if (_Part == EEquipPart.Weapon)
            {
                Atk = _Status;
            }
            else if (_Part == EEquipPart.Armor)
            {
                Def = _Status;
            }
            else
            {
                //Debug.Assert(false);
            }
            IsEquip = false;
        }

        public EEquipPart Part { get; set; }

        public int Atk { get; set; }
        public int Def { get; set; }

        public bool IsEquip { get; set; }
    }
}

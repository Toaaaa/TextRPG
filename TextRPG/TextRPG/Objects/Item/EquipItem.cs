using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum EEquipPart
{
    None,
    Weapon,
    Armor,
}

namespace TextRPG.Item
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
        }

        public EEquipPart Part { get; private set; }

        public int Atk { get; private set; }
        public int Def { get; private set; }

        public bool IsEquip { get; set; }
    }
}

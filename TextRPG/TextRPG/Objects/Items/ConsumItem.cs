using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EConsumItem
{
    None,
    Potion,
}

namespace TextRPG.Objects.Items
{
    public class ConsumItem : Item
    {
        public ConsumItem()
        { 
        }

        public ConsumItem(EConsumItem _Category)
        {
            Category = _Category;
            Num = 1;
        }

        //public
        public EConsumItem Category { get; set; }

        public int Num { get; set; }

        //public
        public void UseItem(Player _player, EConsumItem _Category)
        {
            switch (_Category)
            {
                case EConsumItem.Potion:
                    //_player.HP + 10;
                    // if(_player.HP > _player.MaxHP) 와 같은 확인 필요할듯
                    break;
                default:
                    break;
            }
        }
    }
}

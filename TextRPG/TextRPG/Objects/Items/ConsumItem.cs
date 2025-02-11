using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public void UseItem(Player _Player, EConsumItem _Category)
        {
            switch (_Category)
            {
                case EConsumItem.Potion:
                    switch (Name)
                    {
                        case "포션":
                            _Player.HP += 30;
                            
                            break;
                        case "하이 포션":
                            _Player.HP += 50;
                            break;
                        default:
                            Logger.Debug("존재하지 않는 포션 이름입니다.");
                            break;
                    }

                    if (_Player.HP > _Player.MaxHP)
                    {
                        _Player.HP = _Player.MaxHP;
                    }
                    break;
                default:
                    Logger.Debug("존재하지 않는 아이템 유형입니다.");
                    break;
            }
        }
    }
}

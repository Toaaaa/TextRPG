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

namespace TextRPG.Item
{
    public class ConsumItem : Item
    {
        public ConsumItem(EConsumItem _Category) 
        {
            Category = _Category;
        }

        public EConsumItem Category;

        public int Num = 1;

        public void UseItem(EConsumItem _Category)
        {
            switch (_Category)
            {
                
            }
        }
    }
}

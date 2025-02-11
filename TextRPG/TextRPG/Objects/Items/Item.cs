using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG.Objects.Items
{
    public class Item
    {
        // 생성자
        public Item() { }

        //public
        public string Name { get; set; }
        public string Explain { get; set; }
        public int Price { get; set; }
    }
}

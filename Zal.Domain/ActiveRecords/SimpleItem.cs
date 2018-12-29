using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.ActiveRecords
{
    public class SimpleItem:ISimpleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public SimpleItem(string title, string text) {
            Id = 0;
            Title = title;
            Text = text;
        }

        public SimpleItem(string title) : this(title, "") {
        }
    }
}

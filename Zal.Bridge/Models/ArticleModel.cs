using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models
{
    public class ArticleModel : IModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Type { get; set; }
        public int Id_Author { get; set; }
        public DateTime Date { get; set; }
        public int? Id_Gallery { get; set; }
        //public int? Pro_druzinu { get; set; }
    }
}

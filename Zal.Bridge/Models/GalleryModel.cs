using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    [Obsolete]
    public class GalleryModel : IModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string MainImg { get; set; }
        public DateTime Date { get; set; }
    }
}

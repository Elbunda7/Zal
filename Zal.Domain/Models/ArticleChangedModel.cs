using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Models
{
    public class ArticleChangedModel : BaseChangedActiveRecords<Article>
    {
        public int[] Ids { get; set; }

        public ArticleChangedModel(ArticlesChangesRespondModel rawModel, IEnumerable<Article> activeRecords) : base(rawModel, activeRecords) {
            Ids = rawModel.Ids;
        }
    }
}

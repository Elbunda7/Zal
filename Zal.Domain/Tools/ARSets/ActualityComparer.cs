using Zal.Domain.ActiveRecords;
using System.Collections.Generic;
using static Zal.Domain.Consts.ZAL;
using System;

namespace Zal.Domain.Tools.ARSets
{
    public class ActualityComparer : Comparer<Article>
    {
        public override int Compare(Article x, Article y) {                     
            int comparison = Comparer<long>.Default.Compare(GetOrderValue(y), GetOrderValue(x));
            if (comparison == 0) {
                comparison = Comparer<int>.Default.Compare(y.Id, x.Id);
            } 
            return comparison;
        }

        private long GetOrderValue(Article article) {
            long value;
            if (article.Type == ArticleType.Info) {
                value = DateTime.Now.Ticks - (article.Date.Ticks - DateTime.Now.Ticks) / 2;
            }
            else {
                value = article.Date.Ticks;
            }
            return value;
        }
    }
}

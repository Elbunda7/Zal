using Zal.Domain.ActiveRecords;
using System.Collections.Generic;
using static Zal.Domain.Consts.ZAL;
using System;

namespace Zal.Domain.Tools.ARSets
{
    public class ActualityComparer : Comparer<Article>
    {
        public override int Compare(Article x, Article y) {
            long ticks = DateTime.Now.Ticks;
            long xOrderValue = GetOrderValue(x, ticks);
            long yOrderValue = GetOrderValue(y, ticks);

            int comparison = Comparer<long>.Default.Compare(yOrderValue, xOrderValue);
            if (comparison == 0) {
                comparison = Comparer<int>.Default.Compare(y.Id, x.Id);
            } 
            return comparison;
        }

        private long GetOrderValue(Article article, long ticks) {
            long value;
            if (article.Type == ArticleType.Info) {
                value = ticks - (article.Date.Ticks - ticks) / 2;
            }
            else {
                value = article.Date.Ticks;
            }
            return value;
        }
    }
}

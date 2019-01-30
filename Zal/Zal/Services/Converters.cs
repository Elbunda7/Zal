using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Zal.Domain.Consts;

namespace Zal.Services
{
    public class RankToImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZAL.Rank)
            {
                switch ((ZAL.Rank)value)
                {
                    case ZAL.Rank.Kadet: return "rank_tier1.png";
                    case ZAL.Rank.Podradce: return "rank_tier2.png";
                    case ZAL.Rank.Radce: return "rank_tier3.png";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

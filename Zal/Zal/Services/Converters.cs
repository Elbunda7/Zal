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

    public class GroupToImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZAL.Group)
            {
                switch ((ZAL.Group)value)
                {
                    case ZAL.Group.Lisky: return "";
                    case ZAL.Group.Bobri: return "bobr_40dp.png";
                    case ZAL.Group.Jesterky: return "jesterka_40dp.png";
                    case ZAL.Group.Svisti: return "svist_40dp.png";
                    case ZAL.Group.Veverky: return "veverka_40dp.png";
                    case ZAL.Group.Trosky: return "trosky_40dp.png";
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

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


    public class RankToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZAL.Rank)
            {
                switch ((ZAL.Rank)value)
                {
                    case ZAL.Rank.Kadet:
                    case ZAL.Rank.Podradce:
                    case ZAL.Rank.Radce: return true;
                }
            }
            return false;
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


    public class ArticleTypeToImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZAL.ArticleType)
            {
                switch ((ZAL.ArticleType)value)
                {
                    case ZAL.ArticleType.Article: return "news_80dp.png";
                    case ZAL.ArticleType.Info: return "infoMap_80dp.png";
                    case ZAL.ArticleType.Record: return "note_80dp.png";
                    case ZAL.ArticleType.Link:
                        break;
                    default:
                        break;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrueToAccentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
              return (value is bool && (bool)value) ? Color.Accent : Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FalseToAccentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && !(bool)value) ? Color.Accent : Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BoolNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !(bool)value;
        }
    }
}

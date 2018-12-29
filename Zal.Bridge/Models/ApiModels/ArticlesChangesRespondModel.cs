
namespace Zal.Bridge.Models.ApiModels
{
    public class ArticlesChangesRespondModel : BaseChangesRespondModel<ArticleModel>
    {
        public int[] Ids { get; set; }
    }
}

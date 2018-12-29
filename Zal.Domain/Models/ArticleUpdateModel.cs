using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;

namespace Zal.Domain.Models
{
    public class ArticleUpdateModel : IUpdatableModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int? Id_Gallery { get; set; }

        public void CopyInto(IModel apiModel) {
            ArticleModel model = apiModel as ArticleModel;
            model.Title = Title;
            model.Text = Text;
            model.Id_Gallery = Id_Gallery;
        }

        public void CopyFrom(IModel apiModel) {
            ArticleModel model = apiModel as ArticleModel;
            Title = model.Title;
            Text = model.Text;
            Id_Gallery = model.Id_Gallery;
        }
    }
}

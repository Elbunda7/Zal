
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge;
using Zal.Bridge.Models;

namespace Zal.Domain.ActiveRecords
{
    public class Document : IActiveRecord, ISimpleItem
    {
        private DocumentModel model;

        public int Id => model.Id;
        public string Title => model.Name;
        public string Text => model.Text;

        private static DocumentGateway gateway;
        private static DocumentGateway Gateway => gateway ?? (gateway = new DocumentGateway());

        public Document(DocumentModel model) {
            this.model = model;
        }

        public static async Task<IEnumerable<Document>> GetAll() {
            IEnumerable<DocumentModel> rawModels = await Gateway.GetAllAsync();
            IEnumerable<Document> badges = rawModels.Select(model => new Document(model));
            return badges;
        }


        public override string ToString() {
            return model.Name;
        }
    }
}

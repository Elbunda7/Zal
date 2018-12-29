using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zal.Domain.ItemSets
{
    public class DocumentSet
    {
        public Collection<Document> Data { get; private set; }
        private DateTime LastCheck;

        public DocumentSet() {
            Data = new Collection<Document>();
            LastCheck = ZAL.DATE_OF_ORIGIN;
        }

        public Collection<ISimpleItem> GetAllSimple() {
            Collection<ISimpleItem> simple = new Collection<ISimpleItem>();
            foreach (Document doc in Data) {
                simple.Add(doc);
            }
            return simple;
        }

        internal async void Synchronize() {
            if (LastCheck == ZAL.DATE_OF_ORIGIN) {
                Data = await Document.GetAll() as Collection<Document>;
                LastCheck = DateTime.Now;
            }
        }

        internal void ReSynchronize() {
            LastCheck = ZAL.DATE_OF_ORIGIN;
            Synchronize();
        }

        internal XElement GetXml(string elementName) {
            throw new NotImplementedException();
        }

        internal void LoadFromXml(XElement xElement) {
            throw new NotImplementedException();
        }
    }
}

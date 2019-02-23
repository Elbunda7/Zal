using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class GallerySet : BaseSet
    {
        public GalleryObservableSortedSet Data { get; set; }
        private DateTime lastCheck;

        public GallerySet()
        {
            Data = new GalleryObservableSortedSet();
            lastCheck = ZAL.DATE_OF_ORIGIN;
        }

        public async Task Synchronize()
        {
            if (lastCheck == ZAL.DATE_OF_ORIGIN)
            {
                Data = new GalleryObservableSortedSet(await Gallery.GetAll());
                lastCheck = DateTime.Now;            
            }
        }

        internal Task ReSynchronize()
        {
            lastCheck = ZAL.DATE_OF_ORIGIN;
            return Synchronize();
        }

        public async Task<bool> Add(string name, int year, DateTime date)
        {
            var task = Gallery.Add(name, year, date);
            Gallery gallery = await ExecuteTask(task);
            bool isAdded = gallery != null;
            if (isAdded)
            {
                Data.Add(gallery);
            }
            return isAdded;
        }
    }
}

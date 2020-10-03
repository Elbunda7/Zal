using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class GraphGallerySet:BaseSet
    {
        private Dictionary<int, GraphGalleryObservableSortedSet> Data { get; set; }
        private int ShownYear;

        public List<int> Years { get; private set; }

        public GraphGallerySet()
        {
            Data = new Dictionary<int, GraphGalleryObservableSortedSet>();
            Years = new List<int>();
            ShownYear = -1;
        }

        public async Task Synchronize()
        {
            if (Years.Count == 0)
            {
                Data.Clear();
                Years = (await GraphGallery.GetYears()).Select(x => int.Parse(x)).OrderByDescending(x => x).ToList();
            }
        }

        public Task ReSynchronize()
        {
            Years.Clear();
            return Synchronize();
        }

        public async Task<GraphGalleryObservableSortedSet> GetGalleries(int year)
        {
            ShownYear = year;
            if (!Data.ContainsKey(year))
            {
                Data.Add(year, new GraphGalleryObservableSortedSet(await GraphGallery.GetGalleries(year)));//todo execute task
            }
            return Data[year];
        }

        //public async Task<GraphGallery> Add(string name, int year, DateTime date, string mainImgName = "")
        //{
        //    var task = GraphGallery.Add(name, year, date, mainImgName);
        //    GraphGallery gallery = await ExecuteTask(task);
        //    if (gallery != null)
        //    {
        //        Data.Add(gallery);
        //    }
        //    return gallery;
        //}
    }
}

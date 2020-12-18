using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class GraphGallerySet : BaseSet
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

        public async Task<GraphGallery> Add(int year, string name)
        {
            if (!Years.Contains(year))
            {
                if (await GraphGallery.CreateYearFolder(year))
                {
                    Years.Add(year);
                    Data.Add(year, new GraphGalleryObservableSortedSet());
                }
            }
            var gallery = await GraphGallery.CreateGallery(year, name);
            Data[year].Add(gallery);
            return gallery;
        }

        internal JToken GetJson()
        {
            JArray jArray = new JArray();
            foreach (var galsByYear in Data)
            {
                foreach (GraphGallery gal in galsByYear.Value)
                {
                    jArray.Add(gal.GetJson());
                }
            }
            JToken jToken = new JObject{
                {"years", JArray.FromObject(Years) },
                {"items", jArray }
            };
            return jToken;
        }

        internal void LoadFrom(JToken json)
        {
            if (json == null) return;
            var galleries = json.Value<JArray>("items").Select(x => GraphGallery.LoadFrom(x));
            Years = json.Value<JArray>("years").Values<int>().ToList();
            if (galleries.Count() >= 1)
            {
                PlaceEachIntoRelevantCollection(galleries);
            }
        }

        private void PlaceEachIntoRelevantCollection(IEnumerable<GraphGallery> items)
        {
            var usedYears = items.GroupBy(x=>x.Year).Select(x => x.Key);
            foreach (int year in usedYears)
            {
                Data.Add(year, new GraphGalleryObservableSortedSet(items.Where(x => x.Year == year)));
            }
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

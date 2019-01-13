using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class ActualitySet : BaseSet
    {
        public ActualityObservableSortedSet Data { get; set; }
        private int[] TopTenIds => Data.Take(10).Select(x => x.Id).ToArray();

        public ActualitySet() {
            Data = new ActualityObservableSortedSet();
        }

        internal async Task<Article> CreateNewArticle(string title, string text, int fromRank, ZAL.ArticleType type, int? bindToAction = null, int? forGroup = null) {
            //token uživatele            
            return await CreateNewArticle(Zalesak.Session.CurrentUser, title, text, fromRank, type, forGroup, bindToAction);
        }

        internal async Task<Article> CreateNewArticle(User author, string title, string text, int fromRank, ZAL.ArticleType type, int? bindToAction = null, int? forGroup = null) {
            var task = Article.AddAsync(author, title, text, type);//, fromRank, forGroup));
            Article article = await ExecuteTask(task);
            if (article != null) {
                Data.Add(article);
                return article;
            }
            return null;
        }

        public async Task<bool> AddNewArticle(string title, string text, int fromRank, int? forGroup = null) {
            Article article = await CreateNewArticle(Zalesak.Session.CurrentUser, title, text, fromRank, ZAL.ArticleType.Article, forGroup);
            return article != null;
        }

        public async void Remove(Article item) {
            //if (Zal.Me.IsLeader()) { }
            var task = Article.Delete(item);
            if (await ExecuteTask(task)) {
                Data.Remove(item);
            }
            else {
                throw new Exception("Nejde odstranit");
            }
        }

        public async Task Synchronize() {
            var task = Article.LoadTopTen(TopTenIds, Data.LastSynchronization);
            ArticleChangedModel respond = await ExecuteTask(task);
            if (respond.IsChanged) {
                var idsToDelete = TopTenIds.Except(respond.Ids);
                Data.RemoveByIds(idsToDelete.ToArray());
                Data.AddOrUpdateAll(respond.Changed);
                Data.LastSynchronization = respond.Timestamp;
            }
        }

        internal async Task ReSynchronize() {
            Data.Clear();
            await Synchronize();
        }

        public async Task LoadNext() {
            int lastNonInfoId = Data.Last(x => x.Type != ZAL.ArticleType.Info).Id;
            var task = Article.LoadNext(lastNonInfoId);
            Data.AddAll(await ExecuteTask(task));
        }

        public async Task<Article> GetArticleAsync(int id) {
            Article a;
            if (Data.Any(article => article.Id == id)) {
                a = Data.Single(article => article.Id == id);
            }
            else {
                var task = Article.GetAsync(id);
                a = await ExecuteTask(task);
                Data.Add(a);
            }
            return a;
        }

        //public IActualityItem Get(Article a) {
        //    return a.ItemLazyLoad();
        //}

        public IEnumerable<Article> GetAll() {
            Synchronize();
            return Data;
        }

        internal JToken GetJson()
        {
            var toStore = Data.Take(10).Select(x => x.GetJson());
            JArray jArray = new JArray(toStore);
            return jArray;
        }

        internal void LoadFrom(JToken json)
        {
            var actualities = json.Select(x => Article.LoadFrom(x));
            Data.AddAll(actualities);
        }
    }
}
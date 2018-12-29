using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.Domain.Tools.ARSets;
using static Zal.Domain.Consts.ZAL;

namespace Zal.Domain.ItemSets
{
    public class ActualitySet
    {
        public ActualityObservableSortedSet Data { get; set; }
        private int[] topTenIds;

        public ActualitySet() {
            Data = new ActualityObservableSortedSet();
            topTenIds = new int[0];
        }

        internal async Task<Article> CreateNewArticle(string title, string text, int fromRank, ArticleType type, int? bindToAction = null, int? forGroup = null) {
            //token uživatele
            return await CreateNewArticle(Zalesak.Session.CurrentUser, title, text, fromRank, type, forGroup, bindToAction);
        }

        internal async Task<Article> CreateNewArticle(User author, string title, string text, int fromRank, ArticleType type, int? bindToAction = null, int? forGroup = null) {
            Article article = await Article.AddAsync(author, title, text, type);//, fromRank, forGroup));
            if (article != null) {
                Data.Add(article);
                return article;
            }
            return null;
        }

        public async Task<bool> AddNewArticle(string title, string text, int fromRank, int? forGroup = null) {
            Article article = await CreateNewArticle(Zalesak.Session.CurrentUser, title, text, fromRank, ArticleType.Article, forGroup);
            return article != null;
        }

        public async void Remove(Article item) {
            //if (Zal.Me.IsLeader()) { }
            if (await Article.Delete(item)) {
                Data.Remove(item);
            }
            else {
                throw new Exception("Nejde odstranit");
            }
        }

        public async Task Synchronize() {
            ArticleChangedModel respond = await Article.LoadTopTen(topTenIds, Data.LastSynchronization);
            if (respond.IsChanged) {
                var idsToDelete = topTenIds.Except(respond.Ids);
                Data.RemoveByIds(idsToDelete.ToArray());
                Data.AddOrUpdateAll(respond.Changed);
                Data.LastSynchronization = respond.Timestamp;
            }
        }

        internal async Task ReSynchronize() {
            topTenIds = new int[0];
            Data.Clear();
            await Synchronize();
        }

        public async Task LoadNext() {
            int lastNonInfoId = Data.Last(x => x.Type != ArticleType.Info).Id;
            Data.AddAll(await Article.LoadNext(lastNonInfoId));
        }

        public async Task<Article> GetArticleAsync(int id) {
            Article a;
            if (Data.Any(article => article.Id == id)) {
                a = Data.Single(article => article.Id == id);
            }
            else {
                a = await Article.GetAsync(id);
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

        internal XElement GetXml(string elementName) {
            XElement element = new XElement(elementName);
            foreach (Article a in Data) {
                element.Add(a.GetXml("Actuality"));
            }
            return element;
        }

        internal void LoadFromXml(XElement element) {
            IEnumerable<XElement> data = element.Elements("Actuality");
            foreach (XElement el in data) {
                Article actuality = Article.LoadFromXml(el);
                if (!Data.Contains(actuality)) { //contains vs containsById
                    Data.Add(Article.LoadFromXml(el));
                }
            }
        }
    }
}
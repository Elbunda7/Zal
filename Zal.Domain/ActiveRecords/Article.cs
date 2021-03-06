﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Zal.Domain.Tools;
using Zal.Domain.Consts;
using Zal.Bridge.Models;
using Zal.Bridge;
using System.Threading.Tasks;
using Zal.Domain.Models;
using Zal.Bridge.Models.ApiModels;
using Newtonsoft.Json.Linq;

namespace Zal.Domain.ActiveRecords
{
    public class Article : IActiveRecord, ISimpleItem {

        private ArticleModel model;
        private User author;

        public int Id => model.Id;
        public string Title => model.Title;
        public string Text => model.Text;
        public ZAL.ArticleType Type => (ZAL.ArticleType)model.Type;
        public int Id_Author => model.Id_Author;
        public DateTime Date => model.Date;
        public int? Id_Gallery => model.Id_Gallery;
        //public string ShortText { get { return model.ShortText; } }
        //public string Type { get { return GetItemType(); } }        //zrušit 3x null-ids, zařídit typ+id
        /*public int RankLeast { get { return Data.Od_hodnosti; } }
        public int? ForGroup { get { return Data.Pro_druzinu; } }*/

        private static ArticleGateway gateway;
        private static ArticleGateway Gateway => gateway ?? (gateway = new ArticleGateway());

        private UnitOfWork<ArticleUpdateModel> unitOfWork;
        public UnitOfWork<ArticleUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<ArticleUpdateModel>(model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited() {
            return Gateway.UpdateAsync(model, Zalesak.Session.Token);
        }

        private async Task<User> AuthorLazyLoad() {
            if (author == null) {
                author = await Zalesak.Users.Get(Id_Author);
            }
            return author;
        }

        public static async Task<Article> AddAsync(User author, string title, string text, ZAL.ArticleType type, int? bindToAction = null) {
            ArticleModel model = new ArticleModel {
                Id_Author = author.Id,
                Title = title,
                Text = text,
                Type = (int)type,
                Date = DateTime.Now,
            };
            bool respond;
            if (bindToAction.HasValue) {
                (model as ExtendedArticleModel).Id_Action = bindToAction.Value;
                respond = await Gateway.AddAsync(model as ExtendedArticleModel);
            }
            else {
                respond = await Gateway.AddAsync(model);
            }
            if (respond) {
                return new Article(model);
            }
            return null;
        }

        public Article(ArticleModel model) {
            this.model = model;
        }

        public static async Task<string> CheckForChanges(User user, DateTime lastCheck) {
            throw new NotImplementedException();
        }

        public static async Task<ArticleChangedModel> LoadTopTen(int[] ids, DateTime timestamp) {
            var requestModel = new ArticleTopTenRequestModel {
                Ids = ids,
                Timestamp = timestamp,
            };
            var rawRespondModel = await Gateway.LoadIfChangedTopTenAsync(requestModel, "str");
            var changedItems = rawRespondModel.Changed.Select(x => new Article(x));
            return new ArticleChangedModel(rawRespondModel, changedItems);
        }

        public static async Task<IEnumerable<Article>> LoadNext(int lastNonInfoId) {
            var respondModel = await Gateway.LoadNextAsync(lastNonInfoId);
            return respondModel.Select(x => new Article(x));
        }

        public static async Task<Collection<Article>> GetAllFor(int userRank) {
            /* Collection<AktualityTable> actualityValues = Gateway.SelectAllGeneralFor(user.Email);
             Collection<Actuality> actualities = new Collection<Actuality>();
             foreach (AktualityTable a in actualityValues) {
                 actualities.Add(new Actuality(a));
             }
             return actualities;*/
            throw new NotImplementedException();
        }

        public static async Task<List<int>> GetChanged(User user, DateTime lastCheck) {
            throw new NotImplementedException();
        }

        public static async Task<Article> GetAsync(int id) {
            return new Article(await Gateway.GetAsync(id));
        }

        public static async Task<bool> Delete(Article actuality) {
            UserPermision.HasRank(Zalesak.Session.CurrentUser, ZAL.Rank.Vedouci);
            return await Gateway.DeleteAsync(actuality.model.Id);
        }

        internal JToken GetJson()
        {
            return JObject.FromObject(model);
        }

        internal static Article LoadFrom(JToken json)
        {
            var model = json.ToObject<ArticleModel>();
            return new Article(model);
        }
    }
}

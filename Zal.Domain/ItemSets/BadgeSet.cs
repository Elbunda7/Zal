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
    public class BadgeSet
    {
        public IEnumerable<Badge> Badges { get; private set; }
        private DateTime lastCheck;

        public BadgeSet() {
            Badges = new List<Badge>();
            lastCheck = ZAL.DATE_OF_ORIGIN;
        }

        public async Task Synchronize() {
            var respond = await Badge.IfNeededGetAllAsync(Zalesak.Session.UserRank, lastCheck);
            if (respond.IsChanged) {
                lastCheck = respond.Timestamp;
                Badges = respond.Changed;
            }
        }

        internal Task ReSynchronize() {
            lastCheck = ZAL.DATE_OF_ORIGIN;
            return Synchronize();
        }

        public async Task<bool> Add(string name, string text, string image) {
            Badge badge = await Badge.Add(name, text, image);
            bool isAdded = badge != null;
            if (isAdded) {
                (Badges as List<Badge>).Add(badge);
            }
            return isAdded;
        }

        //internal Collection<Badge> GetAcquired() {
        //    return GetAcquired(Zal.Me);
        //}

        internal async Task<IEnumerable<Badge>> GetAcquired(User user) {//vrátit jen idčka
            return await Badge.GetAcquiredAsync(user);
        }

        internal XElement GetXml(string elementName) {
            throw new NotImplementedException();
        }

        internal void LoadFromXml(XElement xElement) {
            throw new NotImplementedException();
        }

        public Badge GetByName(string value) {
            foreach (Badge badge in Badges) {
                if (badge.Title == value) {
                    return badge;
                }
            }
            return Badges.Last();
        }
    }
}

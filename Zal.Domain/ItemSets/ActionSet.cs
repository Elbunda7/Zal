using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class ActionSet : BaseSet {

        public ActionObservableSortedSet UpcomingActionEvents { get; set; }
        private Dictionary<int, ActionObservableSortedSet> ActionEventsDict { get; set; } 
        private int ShownYear;

        public ActionSet() {
            UpcomingActionEvents = new ActionObservableSortedSet();
            ActionEventsDict = new Dictionary<int, ActionObservableSortedSet>();
            ShownYear = DateTime.Today.Year;
        }

        public async Task<ActionObservableSortedSet> GetPassedActionEventsByYear(int year) {
            ShownYear = year;
            if (!ActionEventsDict.ContainsKey(year)) {
                await SynchronizePassedActions();
            }
            return ActionEventsDict[year];
        }

        //když vytvořím 2 stejné akce v offline režimu, tak se navzájem vyruší    

        public async Task ReSynchronizeAsync() {
            UpcomingActionEvents.Clear();
            ActionEventsDict.Clear();
            await SynchronizeAllCurrentlyActive();
        }

        public async Task SynchronizeAllCurrentlyActive() {
            await SynchronizeUpcomingActions();
            await SynchronizePassedActions();
        }

        public async Task SynchronizeUpcomingActions() {
            UpcomingActionEvents = await Synchronize(UpcomingActionEvents, ZAL.YEAR.UPCOMING);
        }

        public async Task SynchronizePassedActions() {
            AddToDictionaryIfNeeded(ShownYear);
            ActionEventsDict[ShownYear] = await Synchronize(ActionEventsDict[ShownYear], ShownYear);
        }

        private Task<ActionObservableSortedSet> Synchronize(ActionObservableSortedSet actions, int year) {
            if (actions.LastSynchronization == ZAL.DATE_OF_ORIGIN) {
                return LoadActionsByYear(actions, year);
            }
            else {
                return LoadChangesByYear(actions, year);
            }
        }

        private async Task<ActionObservableSortedSet> LoadActionsByYear(ActionObservableSortedSet actions, int year) {
            var task = ActionEvent.GetActionsByYear((int)Zalesak.Session.UserRank, year);
            var respond = await ExecuteTask(task);
            return new ActionObservableSortedSet(respond.ActiveRecords) {
                LastSynchronization = respond.Timestamp
            };
        }

        private async Task<ActionObservableSortedSet> LoadChangesByYear(ActionObservableSortedSet actions, int year) {
            var task = ActionEvent.GetChangedAsync((int)Zalesak.Session.UserRank, actions.LastSynchronization, year, actions.Count);
            var respond = await ExecuteTask(task);
            if (respond.IsHardChanged) {
                actions.Clear();
                actions.AddAll(respond.Changed);
                actions.LastSynchronization = respond.Timestamp;
            }
            else if (respond.IsChanged) {
                actions.RemoveByIds(respond.Deleted);
                actions.AddOrUpdateAll(respond.Changed);
                actions.LastSynchronization = respond.Timestamp;
            }
            return actions;
        }

        public async Task<bool> AddNewActionAsync(string name, string type, DateTime dateFrom, DateTime dateTo, int fromRank, bool isOfficial) {
            bool isAdded = false;
            var task = ActionEvent.AddAsync(name, type, dateFrom, dateTo, fromRank, isOfficial);
            ActionEvent item = await ExecuteTask(task);
            if (item != null) {
                PlaceIntoRelevantCollection(item);
                isAdded = true;
            }
            return isAdded;
        }

        private void PlaceIntoRelevantCollection(ActionEvent item) {
            if (item.DateTo >= DateTime.Now) {
                UpcomingActionEvents.Add(item);
            }
            else {
                AddToDictionaryIfNeeded(item.DateFrom.Year);
                ActionEventsDict[item.DateFrom.Year].Add(item);
            }
        }

        private void PlaceEachIntoRelevantCollection(IEnumerable<ActionEvent> items, DateTime timestamp)
        {
            UpcomingActionEvents.AddOrUpdateAll(items.Where(x => x.DateTo >= DateTime.Now));
            UpcomingActionEvents.LastSynchronization = timestamp;
            var usedYears = items.Where(x => x.DateTo < DateTime.Now).Select(x => x.DateFrom.Year);
            foreach (int year in usedYears)
            {
                AddToDictionaryIfNeeded(year);
                ActionEventsDict[year].AddOrUpdateAll(items.Where(x => x.DateTo < DateTime.Now && x.DateFrom.Year == year));
                ActionEventsDict[year].LastSynchronization = timestamp;
            }
        }

        private void AddToDictionaryIfNeeded(int year) {
            if (!ActionEventsDict.ContainsKey(year)) {
                ActionEventsDict.Add(year, new ActionObservableSortedSet());
            }
        }

        internal JToken GetJson()
        {
            JArray jArray = new JArray();
            if (ActionEventsDict.ContainsKey(DateTime.Today.Year))
            {
                foreach (ActionEvent a in ActionEventsDict[DateTime.Today.Year])
                {
                    jArray.Add(a.GetJson());
                }
            }
            foreach (ActionEvent a in UpcomingActionEvents)
            {
                jArray.Add(a.GetJson());
            }
            JToken jToken = new JObject{
                {"timestamp", UpcomingActionEvents.LastSynchronization },
                {"items", jArray }
            };
            return jToken;
        }

        internal void LoadFrom(JToken json)
        {
            var actions = json.Value<JArray>("items").Select(x => ActionEvent.LoadFrom(x));
            var timestamp = json.Value<DateTime>("timestamp");
            if (actions.Count() >= 1)
            {
                PlaceEachIntoRelevantCollection(actions, timestamp);
            }
        }

        public async Task<bool> DeleteAsync(ActionEvent akce)
        {
            bool isDeleted = await akce.DeleteAsync();
            if (isDeleted)
            {
                ActionEventsDict[akce.DateFrom.Year].Remove(akce);
                UpcomingActionEvents.Remove(akce);
            }
            return isDeleted;
        }
    }
}

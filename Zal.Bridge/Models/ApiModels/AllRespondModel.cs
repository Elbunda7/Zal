using System;
using System.Collections.Generic;

namespace Zal.Bridge.Models.ApiModels
{
    public class AllRespondModel<T> where T : IModel
    {
        public DateTime Timestamp { get; set; }
        public IEnumerable<T> Items { get; set; }

        public IEnumerable<T> GetItems() {
            return Items ?? new List<T>();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Models
{
    public class AllActiveRecords<T> where T : IActiveRecord
    {
        public DateTime Timestamp { get; set; }
        public IEnumerable<T> ActiveRecords { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class GraphShareLinkModel
    {
        public string id { get; set; }
        public List<string> roles { get; set; }
        public Link link { get; set; }
        public bool hasPassword { get; set; }
    }

    public class Link
    {
        public string type { get; set; }
        public string scope { get; set; }
        public string webUrl { get; set; }
        public Application application { get; set; }
    }

    public class Application
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }
}

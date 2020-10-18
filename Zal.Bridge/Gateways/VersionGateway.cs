using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Tools;

namespace Zal.Bridge.Gateways
{
    public class VersionGateway : Gateway
    {
        public VersionGateway() : base(API.ENDPOINT.VERSION) { }

        public Task<VersionModel> GetCurrentVersion()
        {
            return SendRequestFor<VersionModel>(API.METHOD.NON);
        }
    }
}

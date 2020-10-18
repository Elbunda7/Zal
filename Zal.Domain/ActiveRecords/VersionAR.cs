using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;

namespace Zal.Domain.ActiveRecords
{
    public static class VersionAR
    {
        private static VersionGateway gateway;
        private static VersionGateway Gateway => gateway ?? (gateway = new VersionGateway());

        public static async Task<Version> GetCurrentVersion()
        {
            var respond = await Gateway.GetCurrentVersion();
            Version v = new Version(respond.Version);
            return v;
        }
    }
}

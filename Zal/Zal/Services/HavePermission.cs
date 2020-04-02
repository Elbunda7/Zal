using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Services
{
    public static class HavePermission
    {
        public async static Task<bool> For<TPermission>() where TPermission : Permissions.BasePlatformPermission, new()
        {
            var status = await Permissions.CheckStatusAsync<TPermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<TPermission>();
            }
            return status == PermissionStatus.Granted;
        }
    }
}

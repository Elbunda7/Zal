using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Services
{
    public static class HavePermission
    {
        public async static Task<bool> For(Permission permission)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                var dictionary = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                status = dictionary[permission];
            }
            return status == PermissionStatus.Granted;
        }
    }
}

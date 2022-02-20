
using System;
using System.Reflection;
using MonoDevelop.Core;

namespace MonoDevelop.DotNetCore.Extensions
{
    static class DotNetCoreSdkExtension
    {
        public static void Update(DotNetCoreSdkPathsExtension sdkPaths)
        {
            if (sdkPaths.DotNetCoreSdkPathsInstance == null)
            {
                LoggingService.LogError("Could not call DotNetCoreSdk.Update via reflection since DotNetCoreSdkPathsInstance is null");
                return;
            }

            Type type = typeof(DotNetCoreSdk);
            MethodInfo? method = type.GetMethod("Update", BindingFlags.Static | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(null, new object[] { sdkPaths.DotNetCoreSdkPathsInstance });
            }
            else
            {
                LoggingService.LogError("Could not call DotNetCoreSdk.Update via reflection");
            }
        }
    }
}

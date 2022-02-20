
using System;
using System.Reflection;
using MonoDevelop.Core;

namespace MonoDevelop.DotNetCore.Extensions
{
    class DotNetCoreSdkPathsExtension
    {
        public DotNetCoreSdkPathsExtension(string dotNetCorePath, bool initializeSdkLocation)
        {
            try
            {
                DotNetCoreSdkPathsInstance = typeof(DotNetCoreRuntime).Assembly.CreateInstance(
                    "MonoDevelop.DotNetCore.DotNetCoreSdkPaths",
                    false,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new object[] { dotNetCorePath, initializeSdkLocation },
                    null,
                    new object[0]);
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Could not find DotNetCoreSdkPaths via reflection", ex);
                return;
            }

            if (DotNetCoreSdkPathsInstance == null)
            {
                LoggingService.LogError("Could not find DotNetCoreSdkPaths via reflection");
            }
        }

        public object? DotNetCoreSdkPathsInstance { get; }

        public void ResolveSDK(string workingDir = "", bool forceLookUpGlobalJson = false)
        {
            if (DotNetCoreSdkPathsInstance == null)
            {
                return;
            }

            Type type = DotNetCoreSdkPathsInstance.GetType();
            MethodInfo? method = type.GetMethod("ResolveSDK", BindingFlags.Instance | BindingFlags.Public);
            if (method != null)
            {
                method.Invoke(DotNetCoreSdkPathsInstance, new object[] { workingDir, forceLookUpGlobalJson });
            }
            else
            {
                LoggingService.LogError("Could not find DotNetCoreSdkPaths.ResolveSDK method via reflection");
            }
        }
    }
}

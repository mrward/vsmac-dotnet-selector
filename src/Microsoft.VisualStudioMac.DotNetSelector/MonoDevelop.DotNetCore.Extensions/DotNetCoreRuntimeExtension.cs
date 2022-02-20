
using System;
using System.Reflection;
using MonoDevelop.Core;

namespace MonoDevelop.DotNetCore.Extensions
{
    static class DotNetCoreRuntimeExtension
    {
        public static void Init(DotNetCorePathExtension path)
        {
            if (path.DotNetCorePathInstance == null)
            {
                LoggingService.LogError("Could not call DotNetCoreRuntime.Init via reflections since DotNetCorePath is null");
                return;
            }

            Type type = typeof(DotNetCoreRuntime);
            MethodInfo? method = type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(null, new object[] { path.DotNetCorePathInstance });
            }
            else
            {
                LoggingService.LogError("Could not call DotNetCoreRuntime.Init via reflection");
            }
        }
    }
}

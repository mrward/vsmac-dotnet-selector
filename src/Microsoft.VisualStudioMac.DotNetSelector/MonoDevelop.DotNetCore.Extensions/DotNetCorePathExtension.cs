
using System;
using MonoDevelop.Core;

namespace MonoDevelop.DotNetCore.Extensions
{
    class DotNetCorePathExtension
    {
        public static readonly string DefaultDotNetCorePath = "/usr/local/share/dotnet/dotnet";

        string fileName;

        public DotNetCorePathExtension(string fileName)
        {
            this.fileName = fileName;

            Type? type = typeof(DotNetCoreRuntime).Assembly.GetType("MonoDevelop.DotNetCore.DotNetCorePath");

            if (type == null)
            {
                LoggingService.LogError("Could not find DotNetCorePath type");
            }

            try
            {
                DotNetCorePathInstance = Activator.CreateInstance(type, fileName);
            }
            catch (Exception ex)
            {
                LoggingService.LogError("Could not find DotNetCorePath via reflection", ex);
            }

            if (DotNetCorePathInstance == null)
            {
                LoggingService.LogError("Could not find DotNetCorePath via reflection");
            }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public object? DotNetCorePathInstance { get; }
    }
}


using System;
using System.Threading.Tasks;
using MonoDevelop.Core;
using MonoDevelop.DotNetCore;
using MonoDevelop.DotNetCore.Extensions;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace Microsoft.VisualStudioMac.DotNetSelector
{
    class DotNetLocationUpdater
    {
        public DotNetLocationUpdater()
        {
        }

        public Task ChangeLocationAsync(DotNetLocation location)
        {
            ChangeLocation(location);

            return ReevaluateAllOpenDotNetCoreProjects();
        }

        void ChangeLocation(DotNetLocation location)
        {
            FilePath dotNetFileName = location.RootDirectory.Combine("dotnet");

            var sdkPaths = new DotNetCoreSdkPathsExtension(dotNetFileName, initializeSdkLocation: true);
            sdkPaths.ResolveSDK();

            DotNetCoreSdkExtension.Update(sdkPaths);

            var path = new DotNetCorePathExtension(dotNetFileName);
            DotNetCoreRuntimeExtension.Init(path);
        }

        static async Task ReevaluateAllOpenDotNetCoreProjects()
        {
            if (!IdeApp.Workspace.IsOpen)
            {
                return;
            }

            var progressMonitor = new ProgressMonitor();
            foreach (var project in IdeApp.Workspace.GetAllItems<DotNetProject>())
            {
                if (project.HasFlavor<DotNetCoreProjectExtension>())
                {
                    await project.ReevaluateProject(progressMonitor);
                }
            }
        }
    }
}


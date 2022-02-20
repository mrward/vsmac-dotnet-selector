
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoDevelop.Core;
using MonoDevelop.DotNetCore;

namespace Microsoft.VisualStudioMac.DotNetSelector
{
    class DotNetLocator
    {
        static FilePath DefaultDotNetRootPath = "/usr/local/share/dotnet";
        static FilePath DefaultDotNetFileName = "/usr/local/share/dotnet/dotnet";
        const string DotNetCoreRuntimeFileNameProperty = "DotNetCoreRuntimeFileName";

        SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        ImmutableArray<DotNetLocation> locations;

        public DotNetLocator()
        {
        }

        public bool FoundLocations
        {
            get { return !Locations.IsDefault; }
        }

        public ImmutableArray<DotNetLocation> Locations
        {
            get { return locations; }
        }

        public DotNetLocation? ActiveLocation { get; internal set; }

        public async Task<ImmutableArray<DotNetLocation>>
            GetDotNetLocationsAsync(CancellationToken token = default)
        {
            if (FoundLocations)
            {
                return Locations;
            }

            await locker.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (FoundLocations)
                {
                    return Locations;
                }

                locations = await Task.Run(() => GetDotNetLocations())
                    .ConfigureAwait(false);
            }
            finally
            {
                locker.Release();
            }

            return Locations;
        }

        ImmutableArray<DotNetLocation> GetDotNetLocations()
        {
            var foundLocations = new List<DotNetLocation>();

            if (!DotNetCoreRuntime.IsMissing)
            {
                FilePath fileName = DotNetCoreRuntime.FileName;

                string name = GetDotNetLocationName(fileName);

                var location = new DotNetLocation(name, fileName.ParentDirectory);
                foundLocations.Add(location);

                ActiveLocation = location;
            }

            if (DotNetCoreRuntime.FileName != DefaultDotNetFileName)
            {
                if (SafeFileExists(DefaultDotNetFileName))
                {
                    string name = GettextCatalog.GetString("Default");
                    var location = new DotNetLocation(name, DefaultDotNetRootPath);
                    foundLocations.Add(location);
                }
            }

            try
            {
                FilePath parentDirectory = DefaultDotNetRootPath.ParentDirectory;

                foreach (FilePath directory in Directory.EnumerateDirectories(parentDirectory, "*dotnet*"))
                {
                    FilePath dotNetFileName = directory.Combine("dotnet");
                    if (dotNetFileName == DotNetCoreRuntime.FileName ||
                        dotNetFileName == DefaultDotNetFileName)
                    {
                        // Ignore - already added.
                        continue;
                    }

                    if (SafeFileExists(dotNetFileName))
                    {
                        var location = new DotNetLocation(directory);
                        foundLocations.Add(location);
                    }
                }

            }
            catch (Exception ex)
            {
                LoggingService.LogError("Could search for dotnet locations", ex);
            }

            return foundLocations
                .OrderBy(location => location.Name)
                .ToImmutableArray();
        }

        static string GetDotNetLocationName(FilePath dotNetFileName)
        {
            if (dotNetFileName == DefaultDotNetFileName)
            {
                return GettextCatalog.GetString("Default");
            }

            string fileName = PropertyService.Get<string>(DotNetCoreRuntimeFileNameProperty);
            if (fileName != null && dotNetFileName == fileName)
            {
                return GettextCatalog.GetString("From Preferences");
            }

            return GettextCatalog.GetString("From PATH");
        }

        bool SafeFileExists(string fileName)
        {
            try
            {
                return File.Exists(fileName);
            }
            catch (Exception ex)
            {
                LoggingService.LogError(string.Format("Error checking file exists '{0}'", fileName), ex);
            }

            return false;
        }
    }
}


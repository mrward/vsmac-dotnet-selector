
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MonoDevelop.Components.Commands;

namespace Microsoft.VisualStudioMac.DotNetSelector
{
    class SelectActiveDotNetHandler : CommandHandler
    {
        protected override async Task UpdateAsync(CommandArrayInfo info, CancellationToken cancelToken)
        {
            ImmutableArray<DotNetLocation> locations =
                await DotNetSelectorServices.DotNetLocator.GetDotNetLocationsAsync(cancelToken);

            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            foreach (DotNetLocation location in locations)
            {
                CommandInfo locationInfo = info.Add(location.ToString(), location);
                locationInfo.Checked = location == DotNetSelectorServices.DotNetLocator.ActiveLocation;
            }
        }

        protected override void Run(object dataItem)
        {
            var location = (DotNetLocation)dataItem;

            DotNetSelectorServices.DotNetLocator.ActiveLocation = location;

            DotNetSelectorServices.LocationUpdater.ChangeLocationAsync(location)
                .Ignore();
        }
    }
}


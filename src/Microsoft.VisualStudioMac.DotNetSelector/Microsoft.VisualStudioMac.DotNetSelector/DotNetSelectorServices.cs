
namespace Microsoft.VisualStudioMac.DotNetSelector
{
    static class DotNetSelectorServices
    {
        static DotNetSelectorServices()
        {
            DotNetLocator = new DotNetLocator();
            LocationUpdater = new DotNetLocationUpdater();
        }

        public static DotNetLocator DotNetLocator { get; }
        public static DotNetLocationUpdater LocationUpdater { get; }
    }
}


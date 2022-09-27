using API;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Stores.Component
{
    public class AppSettings
    {
        public APISettings APISettings { get; set; } = new();
        public UXSettings UXSettings { get; set; } = new();
    }
}
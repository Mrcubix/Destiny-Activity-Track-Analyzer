using API;
using Tracker.Shared.Backend;

namespace Tracker.Shared
{
    public class AppSettings
    {
        public APISettings APISettings { get; set; } = new();
        public UXSettings UXSettings { get; set; } = new();
    }
}
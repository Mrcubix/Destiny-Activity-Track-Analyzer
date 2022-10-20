using Tracker.Shared.Frontend;

namespace Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainViewModel(ViewRemote remote)
        {
            Remote = remote;
        }
    }
}

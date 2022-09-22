namespace Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainViewModel()
        {
            Remote.AddVM(this);

            Remote.CurrentViewModel = this;
        }

    }
}

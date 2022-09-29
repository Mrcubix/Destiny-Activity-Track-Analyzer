using ReactiveUI;
using Tracker.Shared.Frontend;

namespace Tracker.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        private ViewRemote remote = null!;

        public ViewRemote Remote
        {
            get => remote;
            set => this.RaiseAndSetIfChanged(ref remote, value);
        }
        
        public ViewModelBase() {}

        public ViewModelBase(ViewModelBase initialVM) 
        {
            remote = new(initialVM);
        }

        public ViewModelBase(ViewRemote remote)
        {
            this.remote = remote;
        }
    }
}

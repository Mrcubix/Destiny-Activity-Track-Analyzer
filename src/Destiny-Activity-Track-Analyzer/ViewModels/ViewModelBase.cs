using ReactiveUI;
using Tracker.Shared.Frontend;

namespace Tracker.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        private string _name = null!;
        private ViewRemote _remote = null!;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public ViewRemote Remote
        {
            get => _remote;
            set => this.RaiseAndSetIfChanged(ref _remote, value);
        }
        
        public ViewModelBase() {}

        public ViewModelBase(ViewModelBase initialVM, string name = "") 
        {
            this.Remote = new(initialVM);

            if (string.IsNullOrEmpty(name))
                this.Name = this.GetType().Name;
            else
                this.Name = name;
        }

        public ViewModelBase(ViewRemote remote, string name = "")
        {
            this.Remote = remote;

            if (string.IsNullOrEmpty(name))
                this.Name = this.GetType().Name;
            else
                this.Name = name;
        }
    }
}

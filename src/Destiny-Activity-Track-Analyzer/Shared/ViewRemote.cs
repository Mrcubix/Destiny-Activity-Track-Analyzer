using System.Collections.ObjectModel;
using Tracker.ViewModels;
using ReactiveUI;

namespace Tracker.Shared.Frontend
{
    public class ViewRemote : ReactiveObject
    {
        private ViewModelBase _currentViewModel = null!;
        private ObservableCollection<string> _viewModelNames = new();
        private ObservableCollection<ViewModelBase> _viewModels = new();
        private SharedStores _sharedStores = null!;

        public ViewRemote() {}

        public ViewRemote(ViewModelBase initialVM)
        {
            CurrentViewModel = initialVM;
        }

        public ViewModelBase CurrentViewModel 
        { 
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }

        private ObservableCollection<string> ViewModelNames 
        { 
            get => _viewModelNames;
            set => this.RaiseAndSetIfChanged(ref _viewModelNames, value);
        }

        public ObservableCollection<ViewModelBase> ViewModels 
        { 
            get => _viewModels;
            set => this.RaiseAndSetIfChanged(ref _viewModels, value);
        }

        public SharedStores SharedStores 
        { 
            get => _sharedStores;
            set => this.RaiseAndSetIfChanged(ref _sharedStores, value);
        }

        public void AddVM(ViewModelBase vm)
        {
            // Duplicates are not allowed
            if (_viewModelNames.Contains(vm.Name))
                return;

            ViewModelNames.Add(vm.Name);
            
            ViewModels.Add(vm);

            if (CurrentViewModel == null)
                CurrentViewModel = vm;
        }

        public void AddVM(ViewModelBase vm, string name)
        {
            // Duplicates are not allowed
            if (_viewModelNames.Contains(name))
                return;

            vm.Name = name;
            
            ViewModels.Add(vm);
            ViewModelNames.Add(name);

            if (CurrentViewModel == null)
                CurrentViewModel = vm;
        }

        public void RemoveVM(ViewModelBase vm)
        {
            ViewModels.Remove(vm);
            ViewModelNames.Remove(vm.Name);
        }

        public void RemoveVMByName(string name)
        {
            ViewModels.Remove(ViewModels[ViewModelNames.IndexOf(name)]);
            ViewModelNames.Remove(name);
        }

        public bool ShowView(string viewName)
        {
            for(int i = 0; i < _viewModelNames.Count; i++)
            {
                if (_viewModelNames[i] == viewName)
                {
                    CurrentViewModel = _viewModels[i];
                    return true;
                }
            }
            return false;
        }
    }
}

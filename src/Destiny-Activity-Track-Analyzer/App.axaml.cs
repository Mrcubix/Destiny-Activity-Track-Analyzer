using System.Threading.Tasks;
using API.Endpoints;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tracker.Shared.Converters;
using Tracker.Shared.Frontend;
using Tracker.ViewModels;
using Tracker.Views;

namespace Tracker
{
    public partial class App : Application
    {
        public ViewRemote Remote { get; set; } = new();
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            MainViewModel context = new MainViewModel(Remote);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = context
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = context
                };
            }

            InitializeBackend(context);
            InitializeFrontend();

            base.OnFrameworkInitializationCompleted();
        }

        public void InitializeBackend(ViewModelBase context)
        {
            Remote.AddVM(context);
            Remote.SharedStores = new(context);
            Remote.SharedStores.Initialize();

            _ = Task.Run(LoadStore);
        }

        public void InitializeFrontend()
        {
            LoadViewModels();
            InitializeConverters();
        }

        public async Task LoadStore()
        {
            var shared = Remote.SharedStores;

            shared.Load();

            await shared.UserStore.Update();

            var defaults = Remote.SharedStores.DefaultsStore.Defaults;

            if (string.IsNullOrEmpty(defaults.DefaultViewModelName))
                defaults.DefaultViewModelName = "Current Activity";

            // this should be done when the remote is init
            if (shared.SettingsStore.IsKeySet && shared.UserStore.IsUserSet)
                if (Remote.SharedStores.UserStore.User.CurrentCharacter != null)
                    Remote.ShowView(Remote.SharedStores.DefaultsStore.Defaults.DefaultViewModelName);
        }

        public void LoadViewModels()
        {
            Remote.AddVM(new CurrentActivityViewModel(Remote, "Current Activity"));
            Remote.AddVM(new SettingsViewModel(Remote, "Settings"));
            Remote.AddVM(new CharacterPickerViewModel(Remote, "Character Picker"));
            Remote.AddVM(new KeyEnquiryViewModel(Remote, "Key Enquiry"));
            Remote.AddVM(new UserEnquiryViewModel(Remote, "User Enquiry"));
        }

        public void InitializeConverters()
        {
            ClassHashConverter.Remote = Remote;
            RaceHashConverter.Remote = Remote;
            CharacterToIndexConverter.Remote = Remote;
            EmblemHashToEmblemBackground.Remote = Remote;
        }
    }
}
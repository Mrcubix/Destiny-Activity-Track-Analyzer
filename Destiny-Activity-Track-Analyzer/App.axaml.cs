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
        public Destiny2 API { get; set; } = null!;
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

            Remote.AddVM(context);
            Remote.SharedStores = new(context);
            Remote.SharedStores.Initialize();

            API = new(Remote.SharedStores.SettingsStore.Settings.APISettings);

            _ = Task.Run(LoadStore);
            LoadViewModels();
            InitializeConverters();

            base.OnFrameworkInitializationCompleted();
        }

        public void LoadViewModels()
        {
            Remote.AddVM(new CurrentActivityViewModel(API, Remote));
            Remote.AddVM(new SettingsViewModel(Remote));

            Remote.ShowView(Remote.SharedStores.SettingsStore.Settings.UXSettings.DefaultViewModelName);
        }

        public async Task LoadStore()
        {
            Remote.SharedStores.Load();

            await Remote.SharedStores.SettingsStore.Update();
        }

        public void InitializeConverters()
        {
            ClassHashConverter.Remote = Remote;
        }
    }
}
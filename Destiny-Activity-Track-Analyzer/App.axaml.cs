using System;
using System.Reflection;
using System.Threading.Tasks;
using API.Endpoints;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

                context = (MainViewModel)desktop.MainWindow.DataContext;
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

            API = new(Remote.SharedStores.SettingsStore.Settings.APISettings);

            _ = Task.Run(LoadRessources);
            LoadViewModels();

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name; // Destiny-Activity-Track-Analyzer

            var pathResmProject = new Uri($"resm:{assemblyName}/Assets");
            var pathAveresProject = new Uri($"averes://{assemblyName}/Assets");

            var resmProject = assets.GetAssets(pathResmProject, null);
            var averesProject = assets.GetAssets(pathAveresProject, null);

            var ressourcesNames = Assembly.GetExecutingAssembly().GetManifestResourceNames(); // ["!AvaloniaResources"]

            var pathResmRessources = new Uri($"resm:{ressourcesNames[0]}/Assets");
            var pathAveresRessources = new Uri($"averes:/{ressourcesNames[0].Replace("!", "")}/Assets");

            var resmRessources = assets.GetAssets(pathResmRessources, null);
            var averesRessources = assets.GetAssets(pathAveresRessources, null);

            base.OnFrameworkInitializationCompleted();
        }

        public void LoadViewModels()
        {
            Remote.AddVM(new CurrentActivityViewModel(API, Remote));
            Remote.AddVM(new SettingsViewModel(Remote));

            Remote.ShowView(Remote.SharedStores.SettingsStore.Settings.UXSettings.DefaultViewModelName);
        }

        public async Task LoadRessources()
        {
            var sharedDefinitionsStore = Remote.SharedStores.DefinitionsStore;
            
            sharedDefinitionsStore.Initialize();
            await sharedDefinitionsStore.LoadDefinitions();
            await Remote.SharedStores.SettingsStore.Refresh();
        }
    }
}
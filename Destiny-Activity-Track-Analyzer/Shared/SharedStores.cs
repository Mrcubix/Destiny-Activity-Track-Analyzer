using System.Collections.Generic;
using System.Threading.Tasks;
using Tracker.Shared.Interfaces;
using Tracker.Shared.Stores;
using Tracker.ViewModels;

namespace Tracker.Shared
{
    public class SharedStores : IStore
    {
        public List<IStore> Stores = new();

        public DefinitionsStore DefinitionsStore { get; set; }
        public SettingsStore SettingsStore { get; set; }
        public DefaultsStore DefaultsStore { get; set; }
        public IconStore IconStore { get; set; }


        public SharedStores()
        {
            SettingsStore = new();
            DefaultsStore = new(SettingsStore);
            DefinitionsStore = new(SettingsStore);
            IconStore = new();
        }

        public SharedStores(ViewModelBase vm)
        {
            SettingsStore = new();
            DefaultsStore = new(SettingsStore, vm);
            DefinitionsStore = new(SettingsStore);
            IconStore = new();
        }


        /// <Summary>
        ///   Method responsible for initializing of stores
        /// </Summary>
        public void Initialize()
        {
            Stores.Add(SettingsStore);
            Stores.Add(DefinitionsStore);
            Stores.Add(IconStore);
            Stores.Add(DefaultsStore);

            foreach(IStore store in Stores)
                store.Initialize();
        }
        
        /// <Summary>
        ///   Method responsible for loading content of stores
        /// </Summary>
        public void Load()
        {
            foreach(IStore store in Stores)
                store.Load();
        }

        /// <Summary>
        ///   Method responsible for saving content of stores
        /// </Summary>
        public void Save()
        {
            foreach(IStore store in Stores)
                store.Save();
        }

        public void Update()
        {
            SettingsStore.Update();
            _ = Task.Run(DefaultsStore.Update);
        }
    }
}
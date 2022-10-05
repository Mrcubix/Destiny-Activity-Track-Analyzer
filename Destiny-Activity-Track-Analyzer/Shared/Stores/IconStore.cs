using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Platform;
using ReactiveUI;
using Tracker.Shared.Interfaces;

namespace Tracker.Shared.Stores
{
    public class IconStore : ReactiveObject, IStore
    {
        private IAssetLoader? AssetLoader { get; set; }
        Dictionary<uint, string> Icons { get; set; } = new();

        public void Initialize()
        {
            AssetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
        }

        public void Load()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name; // Destiny-Activity-Track-Analyzer

            if (AssetLoader == null)
                throw new NullReferenceException("AssetLoader is null");

            var assets = AssetLoader.GetAssets(new Uri($@"avares://{assemblyName}/Assets/Icons"), null);
            
            uint i = 0;
            // TODO: Icons need to be associated with modes
            foreach(Uri asset in assets)
            {
                Icons.Add(i++, asset.LocalPath);
            }
        }

        public void Save() { }
    }
}
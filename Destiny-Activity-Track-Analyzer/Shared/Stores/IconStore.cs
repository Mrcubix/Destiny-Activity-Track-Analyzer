using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using Tracker.Shared.Interfaces;

namespace Tracker.Shared.Stores
{
    public class IconStore : ReactiveObject, IStore
    {
        private static string? AssemblyName { get; set; } = Assembly.GetExecutingAssembly().GetName().Name;
        private IAssetLoader? AssetsLoader { get; set;}


        // Dictionary of ActivityModeType to Icon, stored in Avalonia's AssetLoader
        public Dictionary<int, string> IconPaths { get; set; } = new()
        {
            [0] = $"avares://{AssemblyName}/Assets/Icons/Activities/All.png",
            [2] = $"avares://{AssemblyName}/Assets/Icons/Activities/Story.png",
            [3] = $"avares://{AssemblyName}/Assets/Icons/Activities/Strike.png",
            [4] = $"avares://{AssemblyName}/Assets/Icons/Activities/Raid.png",
            [5] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [6] = $"avares://{AssemblyName}/Assets/Icons/Activities/Patrol.png",
            [7] = $"avares://{AssemblyName}/Assets/Icons/Activities/PVE.png",
            [10] = $"avares://{AssemblyName}/Assets/Icons/Activities/Control.png",
            [12] = $"avares://{AssemblyName}/Assets/Icons/Activities/Clash.png",
            [15] = $"avares://{AssemblyName}/Assets/Icons/Activities/AllDoubles.png",
            [16] = $"avares://{AssemblyName}/Assets/Icons/Activities/Nightfall.png",
            [17] = $"avares://{AssemblyName}/Assets/Icons/Activities/Nightfall.png",
            [18] = $"avares://{AssemblyName}/Assets/Icons/Activities/Strike.png",
            [19] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png",
            [25] = $"avares://{AssemblyName}/Assets/Icons/Activities/Mayhem.png",
            [31] = $"avares://{AssemblyName}/Assets/Icons/Activities/Supremacy.png",
            [32] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [37] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [38] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [39] = $"avares://{AssemblyName}/Assets/Icons/Activities/TrialsOfTheNine.png",
            [40] = $"avares://{AssemblyName}/Assets/Icons/Activities/Social.png",
            [41] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [42] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [43] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png",
            [44] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png",
            [45] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png",
            [46] = $"avares://{AssemblyName}/Assets/Icons/Activities/Nightfall.png",
            [47] = $"avares://{AssemblyName}/Assets/Icons/Activities/Nightfall.png",
            [48] = $"avares://{AssemblyName}/Assets/Icons/Activities/Rumble.png",
            [49] = $"avares://{AssemblyName}/Assets/Icons/Activities/AllDoubles.png",
            [50] = $"avares://{AssemblyName}/Assets/Icons/Activities/AllDoubles.png",
            [51] = $"avares://{AssemblyName}/Assets/Icons/Activities/Clash.png",
            [52] = $"avares://{AssemblyName}/Assets/Icons/Activities/Control.png",
            [53] = $"avares://{AssemblyName}/Assets/Icons/Activities/Supremacy.png",
            [54] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [55] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [56] = $"avares://{AssemblyName}/Assets/Icons/Activities/Mayhem.png",
            [57] = $"avares://{AssemblyName}/Assets/Icons/Activities/Rumble.png",
            [58] = $"avares://{AssemblyName}/Assets/Icons/Activities/HeroicAdventure.png",
            [59] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [60] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [61] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [62] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [63] = $"avares://{AssemblyName}/Assets/Icons/Activities/Gambit.png",
            [64] = $"avares://{AssemblyName}/Assets/Icons/Activities/PVE.png",
            [65] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [66] = $"avares://{AssemblyName}/Assets/Icons/Activities/MissingIcon.png",
            [67] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [68] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png",
            [69] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [70] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [71] = $"avares://{AssemblyName}/Assets/Icons/Activities/Clash.png",
            [72] = $"avares://{AssemblyName}/Assets/Icons/Activities/Clash.png",
            [73] = $"avares://{AssemblyName}/Assets/Icons/Activities/Control.png",
            [74] = $"avares://{AssemblyName}/Assets/Icons/Activities/Control.png",
            [75] = $"avares://{AssemblyName}/Assets/Icons/Activities/GambitPrime.png",
            [76] = $"avares://{AssemblyName}/Assets/Icons/Activities/Reckonning.png",
            [77] = $"avares://{AssemblyName}/Assets/Icons/Activities/Menagerie.png",
            [78] = $"avares://{AssemblyName}/Assets/Icons/Activities/VectorOffensive.png",
            [79] = $"avares://{AssemblyName}/Assets/Icons/Activities/NightmareHunt.png",
            [80] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [81] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [82] = $"avares://{AssemblyName}/Assets/Icons/Activities/Dungeon.png",
            [83] = $"avares://{AssemblyName}/Assets/Icons/Activities/Sundial.png",
            [84] = $"avares://{AssemblyName}/Assets/Icons/Activities/TrialsOfOsiris.png",
            [85] = $"avares://{AssemblyName}/Assets/Icons/Activities/DaresOfEternity.png",
            [86] = $"avares://{AssemblyName}/Assets/Icons/Activities/All.png",
            [87] = $"avares://{AssemblyName}/Assets/Icons/Activities/LostSector.png",
            [88] = $"avares://{AssemblyName}/Assets/Icons/Activities/Crucible.png",
            [89] = $"avares://{AssemblyName}/Assets/Icons/Activities/Control.png",
            [90] = $"avares://{AssemblyName}/Assets/Icons/Activities/IronBanner.png"
        };

        public Dictionary<int, Bitmap?> ActivityIcons { get; } = new Dictionary<int, Bitmap?>();

        public void Initialize()
        {
            AssetsLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
        }

        public void Load()
        {
            if (AssetsLoader == null)
                return;

            // Dictionary of hashed string & Bitmap
            var loadedIcons = new Dictionary<string, Bitmap?>();

            foreach (var entry in IconPaths)
            {
                // check if the icon is already loaded
                if (loadedIcons.ContainsKey(entry.Value))
                {
                    ActivityIcons.Add(entry.Key, loadedIcons[entry.Value]);
                }
                else
                {
                    var icon = new Bitmap(AssetsLoader.Open(new Uri(entry.Value)));

                    ActivityIcons.Add(entry.Key, icon);
                    loadedIcons.Add(entry.Value, icon);
                }
            }

            Console.WriteLine("Loaded " + loadedIcons.Count + " activity icons");

            loadedIcons.Clear();

            Console.WriteLine("TODO: I might want to load them from a file in the future");
        }

        public void Save() { }

        public void Update()
        {
            Console.WriteLine("TODO: I might want to update them using the API in the future");
            // Get the icons from the API
            // Try to get the icon from DestinyActivityModeDefinition.Items.DisplayProperties.Icon
            // if it's missing_icon or the result is 404, use either missing_icon or an icon related to the gamemode (Crucible / PVE)
        }
    }
}
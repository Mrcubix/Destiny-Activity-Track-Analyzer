using System;
using System.IO;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Tracker.Shared.Static;

namespace Tracker.Shared.Stores.Component
{
    public class Emblem : ReactiveObject
    {
        private uint _hash = 0;
        private string _emblemFilePath = string.Empty;
        private string _backgroundFilePath = string.Empty;
        private string _iconFilePath = string.Empty;


        public string Directory {get; set; } = string.Empty;

        public uint Hash
        {
            get => _hash;
            set
            {
                Directory = Path.Combine(SharedPlatformSpecificVariables.EmblemDir, $"{value}");
                EmblemFilePath = Path.Combine(Directory, "emblem.png");
                EmblemBackgroundFilePath = Path.Combine(Directory, "emblem_background.png");
                EmblemIconFilePath = Path.Combine(Directory, "emblem_icon.png");

                this.RaiseAndSetIfChanged(ref _hash, value);
            }
        }
        
        
        /// <summary>
        /// The path to the emblem file, relative to https://www.bungie.net
        /// </summary>
        public string EmblemPath { get; set; } = string.Empty;

        /// <summary>
        /// The path to the emblem background file, relative to https://www.bungie.net
        /// </summary>
        public string EmblemBackgroundPath { get; set; } = string.Empty;

        /// <summary>
        /// The path to the emblem icon file, relative to https://www.bungie.net
        /// </summary>
        public string EmblemIconPath { get; set; } = string.Empty;


        /// <summary>
        /// The path to the emblem file, may be aboslute.
        /// </summary>
        public string EmblemFilePath
        {
            get => _emblemFilePath;
            set => this.RaiseAndSetIfChanged(ref _emblemFilePath, value);
        }

        /// <summary>
        /// The path to the emblem background file, may be aboslute.
        /// </summary>
        public string EmblemBackgroundFilePath
        {
            get => _backgroundFilePath;
            set => this.RaiseAndSetIfChanged(ref _backgroundFilePath, value);
        }

        /// <summary>
        /// The path to the emblem icon file, may be aboslute.
        /// </summary>
        public string EmblemIconFilePath
        {
            get => _iconFilePath;
            set => this.RaiseAndSetIfChanged(ref _iconFilePath, value);
        }
        

        public Bitmap LoadEmblem()
        {
            var res = Load(EmblemFilePath);

            if (res == null)
                return null!;

            return res;
        }

        public Bitmap LoadEmblemBackground()
        {
            var res = Load(EmblemBackgroundFilePath);

            if (res == null)
                return null!;

            return res;
        }

        public Bitmap LoadEmblemIcon()
        {
            var res = Load(EmblemIconFilePath);

            if (res == null)
                return null!;

            return res;
        }

        private Bitmap Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null!;

            try
            {
                return new Bitmap(path);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("Failed to load emblem: The file was not found");
            }
            catch(ArgumentException)
            {
                Console.WriteLine("Failed to load emblem: The file was not an image");
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("Failed to load emblem: You don't have permissions to open the file at the specified location");
            }
            catch(Exception)
            {
                Console.WriteLine("Failed to load emblem: Unknown error");
            }

            return null!;
        }
    }
}
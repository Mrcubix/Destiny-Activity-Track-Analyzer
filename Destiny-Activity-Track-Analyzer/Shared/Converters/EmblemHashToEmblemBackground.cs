using System;
using System.Globalization;
using System.Linq;
using API.Entities.Characters;
using Avalonia.Data.Converters;
using DynamicData;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores.Component;

namespace Tracker.Shared.Converters
{
    public class EmblemHashToEmblemBackground : IValueConverter
    {
        public static ViewRemote Remote { get; set; } = null!;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is uint hash)
            {
                var emblems = Remote.SharedStores.EmblemStore.Emblems;
                
                if (emblems.ContainsKey(hash))
                    return emblems[hash].LoadEmblemBackground();
            }

            return -1;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using DynamicData;
using Tracker.Shared.Frontend;

namespace Tracker.Shared.Converters
{
    public class RaceHashConverter : IValueConverter
    {
        public static ViewRemote Remote { get; set; } = null!;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is uint classRace)
            {
                var items = Remote.SharedStores.DefinitionsStore.RaceDefinitions.Items;

                if (items.ContainsKey(classRace))
                    return items[classRace].DisplayProperties.Name;
            }

            return "Unknown";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string destinyRace)
            {
                return Remote.SharedStores.DefinitionsStore.RaceDefinitions.Items.FirstOrDefault(x => x.Value.DisplayProperties.Name == destinyRace).Key;
            }

            return 0;
        }
    }
}
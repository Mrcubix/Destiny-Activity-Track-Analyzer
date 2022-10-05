using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using DynamicData;
using Tracker.Shared.Frontend;

namespace Tracker.Shared.Converters
{
    public class ClassHashConverter : IValueConverter
    {
        public static ViewRemote Remote { get; set; } = null!;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is uint classHash)
            {
                var items = Remote.SharedStores.DefinitionsStore.ClassDefinitions.Items;

                if (items.ContainsKey(classHash))
                    return items[classHash].DisplayProperties.Name;
            }

            return "Unknown";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string destinyClass)
            {
                return Remote.SharedStores.DefinitionsStore.ClassDefinitions.Items.FirstOrDefault(x => x.Value.DisplayProperties.Name == destinyClass).Key;
            }

            return 0;
        }
    }
}
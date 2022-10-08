using System;
using System.Globalization;
using System.Linq;
using API.Entities.Characters;
using Avalonia.Data.Converters;
using DynamicData;
using Tracker.Shared.Frontend;

namespace Tracker.Shared.Converters
{
    public class CharacterToIndexConverter : IValueConverter
    {
        public static ViewRemote Remote { get; set; } = null!;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DestinyCharacterComponent character)
            {
                var characters = Remote.SharedStores.DefaultsStore.Defaults.Characters;
                return characters.Keys.IndexOf(character.GetCharacterId());
            }

            return -1;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                var characters = Remote.SharedStores.DefaultsStore.Defaults.Characters;

                if (characters.Count > index && index >= 0)
                    return characters.Values.ToArray()[index];
            }

            return null;
        }
    }
}
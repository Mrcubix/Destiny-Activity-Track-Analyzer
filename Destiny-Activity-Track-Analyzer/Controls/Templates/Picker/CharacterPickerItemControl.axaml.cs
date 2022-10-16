using System.Threading.Tasks;
using API.Entities.Characters;
using Avalonia;
using Avalonia.Controls.Primitives;
using Tracker.ViewModels;

namespace Tracker.Controls.Templates.Picker
{
	public class CharacterPickerItemControl : TemplatedControl
	{
		public DestinyCharacterComponent Character
		{
			get => GetValue(CharacterProperty);
			set => SetValue(CharacterProperty, value);
		}

		public static readonly StyledProperty<DestinyCharacterComponent> CharacterProperty =
			AvaloniaProperty.Register<CharacterPickerItemControl, DestinyCharacterComponent>(nameof(Character));
	}
}
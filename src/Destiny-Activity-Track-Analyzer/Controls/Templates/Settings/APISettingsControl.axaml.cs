using API;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Tracker.Controls.Templates.Settings
{
	public class APISettingsControl : TemplatedControl
	{
		// None of this work, this cannot be used inside axaml for some reasons, it's just not detected
		public APISettings Settings
		{
			get => GetValue(SettingsProperty);
			set => SetValue(SettingsProperty, value);
		}

		public static readonly StyledProperty<APISettings> SettingsProperty =
			AvaloniaProperty.Register<APISettingsControl, APISettings>("Settings");
	}
}
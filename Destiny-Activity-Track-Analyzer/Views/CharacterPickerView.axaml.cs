using API.Entities.Characters;
using Avalonia.Controls;
using Tracker.ViewModels;

namespace Tracker.Views
{
    public partial class CharacterPickerView : UserControl
    {
        private bool firstLoad = true;

        public CharacterPickerView()
        {
            InitializeComponent();

            ListBox? listBox = this.Find<ListBox>("Choice");

            if (listBox == null)
                return;

            listBox.SelectionChanged += OnRealSelectionChanged;
        }

        /// <summary>
        /// This is a workaround for an issue withListBox.SelectionChanged being invoked on a default value, which doesn't happen with web applicaations.
        /// </summary>
        public void OnRealSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
                return;

            var listBox = (ListBox)sender;

            if (listBox.SelectedItem == null)
                return;

            var item = (DestinyCharacterComponent)(listBox.SelectedItem);

            var dataContext = (CharacterPickerViewModel?)this.DataContext;
                
            if (!firstLoad)
                dataContext?.OnCharacterSelect(item.GetCharacterId());
            else
                firstLoad = false;
        }
    }
}
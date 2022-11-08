using System.Threading.Tasks;
using API.Endpoints;
using ReactiveUI;
using Tracker.Shared;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores;
using Tracker.Shared.Stores.Component;

namespace Tracker.ViewModels
{
    public abstract class EnquiryViewModelBase : ViewModelBase
    {
        // Exist to cancel spammers
        protected bool isCheckingCredentials = false;
        protected Destiny2 API { get; set; }


        protected SettingsStore _settingsStore = null!;
        protected DefaultsStore _defaultsStore = null!;
        protected UserStore _userStore = null!;
        protected string _currentError = "";
        protected string _errorColor = "";


        /// <summary>
        ///     An Shortcut toward an existing instance of <see cref="SettingsStore"/>
        /// </summary>
        public virtual SettingsStore SettingsStore
        {
            get => _settingsStore;
            set => this.RaiseAndSetIfChanged(ref _settingsStore, value);
        }

        /// <summary>
        ///     An Shortcut toward an existing instance of <see cref="DefaultsStore"/>
        /// </summary>
        public virtual DefaultsStore DefaultsStore
        {
            get => _defaultsStore;
            set => this.RaiseAndSetIfChanged(ref _defaultsStore, value);
        }

        /// <summary>
        ///     An Shortcut toward an existing instance of <see cref="UserStore"/>
        /// </summary>
        public virtual UserStore UserStore
        {
            get => _userStore;
            set => this.RaiseAndSetIfChanged(ref _userStore, value);
        }

        /// <summary>
        ///     The current error message or status
        /// </summary>
        public virtual string CurrentError
        {
            get => _currentError;
            set => this.RaiseAndSetIfChanged(ref _currentError, value);
        }

        /// <summary>
        ///     The color of the current error message or status
        /// </summary>
        public virtual string ErrorColor
        {
            get => _errorColor;
            set => this.RaiseAndSetIfChanged(ref _errorColor, value);
        }

        public EnquiryViewModelBase(ViewRemote remote, string name = "") : base(remote, name)
        {
            API = new(remote.SharedStores.SettingsStore.Settings.APISettings);
            _settingsStore = remote.SharedStores.SettingsStore;
            _defaultsStore = remote.SharedStores.DefaultsStore;
            _userStore = remote.SharedStores.UserStore;

            Initialize();
        }

        /// <summary>
        ///     A method called on initialization <br />
        ///     Subscribe to the <see cref="Tracker.Shared.Stores.SettingsStore.SettingsUpdated" /> event
        /// </summary>
        public virtual void Initialize()
        {
            Remote.SharedStores.StoresLoaded += OnSettingsChange;
        }

        /// <summary>
        ///     Check whether or not the view overriding it should be shown
        /// </summary>
        /// <param name="settings"> An instance of <see cref="Tracker.Shared.Stores.Component.AppSettings" /></param>
        /// <returns>Whether or not the view should be shown</returns>
        public abstract bool ShouldEnquire(SharedStores stores);

        /// <summary>
        ///     Show the overriding view
        /// </summary>
        public abstract void Enquire();

        /// <summary>
        ///     Used to run neccessary checks after saving
        /// </summary>
        public abstract Task Save();

        /// <summary>
        ///     Used to skip parts of the setup
        /// </summary>
        public abstract void Skip();

        /// <summary>
        ///     A method called when the settings are updated <br />
        ///     Used to update the Endpoint instance when settings eventually get loaded or modified
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="settings">An instance of <see cref="Tracker.Shared.Stores.Component.AppSettings" /></param>
        public virtual void OnSettingsChange(object? sender, SharedStores stores)
        {
            this.API = new(SettingsStore.Settings.APISettings);

            if (ShouldEnquire(stores))
                Enquire();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.User;
using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Static;
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


        public virtual SettingsStore SettingsStore
        {
            get => _settingsStore;
            set => this.RaiseAndSetIfChanged(ref _settingsStore, value);
        }

        public virtual DefaultsStore DefaultsStore
        {
            get => _defaultsStore;
            set => this.RaiseAndSetIfChanged(ref _defaultsStore, value);
        }

        public virtual UserStore UserStore
        {
            get => _userStore;
            set => this.RaiseAndSetIfChanged(ref _userStore, value);
        }

        public virtual string CurrentError
        {
            get => _currentError;
            set => this.RaiseAndSetIfChanged(ref _currentError, value);
        }

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

        public virtual void Initialize()
        {
            Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
        }

        public abstract bool ShouldEnquire(AppSettings settings);

        public abstract void Enquire();

        public abstract Task Save();

        public abstract void Skip();

        public virtual void OnSettingsChange(object? send, AppSettings settings)
        {
            this.API = new(settings.APISettings);

            if (ShouldEnquire(settings))
                Enquire();
        }
    }
}

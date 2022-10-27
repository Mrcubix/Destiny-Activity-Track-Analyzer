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
    public class KeyEnquiryViewModel : ViewModelBase
    {
        private ProcessStartInfo psi = new("https://mrcubix.github.io/Destiny-Activity-Track-Analyzer/Wiki/Setup#Creating-A-New-Bungie-Application")
        {
            UseShellExecute = true
        };
        // Exist to cancel spammers
        private bool isCheckingCredentials = false;
        private Destiny2 API { get; set; }


        private SettingsStore _settingsStore = null!;
        private string _currentError = "";
        private string _errorColor = "";


        public SettingsStore SettingsStore
        {
            get => _settingsStore;
            set => this.RaiseAndSetIfChanged(ref _settingsStore, value);
        }

        public string CurrentError
        {
            get => _currentError;
            set => this.RaiseAndSetIfChanged(ref _currentError, value);
        }

        public string ErrorColor
        {
            get => _errorColor;
            set => this.RaiseAndSetIfChanged(ref _errorColor, value);
        }

        public KeyEnquiryViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
            API = new(remote.SharedStores.SettingsStore.Settings.APISettings);
            _settingsStore = remote.SharedStores.SettingsStore;

            Initialize();
        }

        public void Initialize()
        {
            Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
        }

        public bool ShouldEnquire(AppSettings settings)
        {
            return !(Remote.SharedStores.SettingsStore.IsKeySet); // && settings.UXSettings.ShouldEnquire;
        }

        public void Enquire()
        {
            Remote.ShowView("Key Enquiry");
        }

        public async Task Save()
        {
            if (isCheckingCredentials)
                return;

            SettingsStore.Save();

            CurrentError = "Verifying Validity...";
            ErrorColor = "White";
            List<UserInfoCard> users = null!;

            if (SettingsStore.IsKeySet)
            {
                isCheckingCredentials = true;

                // test if the key is valid by making a request
                try
                {
                    users = await API.SearchDestinyPlayerByBungieName("Gess1t", 9111);
                }
                catch(HttpRequestException E)
                {
                    CurrentError = $"Error: {E.Message}";
                    ErrorColor = "Red";
                }
                catch(InvalidCredentialException E)
                {
                    CurrentError = $"Error: {E.Message}";
                    ErrorColor = "Red";
                }
                catch(Exception E)
                {
                    CurrentError = $"Error: {E.Message}";
                    ErrorColor = "Red";
                }

                isCheckingCredentials = false;

                if (users == null)
                    return;
            }
            
            if(users != null)
            {
                // TODO: check if the other user info is set
                // if it is, then skip the rest of the Setup
                // if not, then go to 'Setup View'
                CurrentError = "";
                Remote.ShowView("MainViewModel");
            }
            else
            {
                CurrentError = "Error: Invalid API Key";
                ErrorColor = "Red";
            }
        }

        public void Skip()
        {
            Remote.ShowView("MainViewModel");
        }

        public void OpenWiki()
        {
            switch(SharedPlatformSpecificVariables.Platform.ToString())
            {
                case "Windows":
                    Process.Start(psi);
                    break;
                case "Linux" or "FreeBSD":
                    Process.Start("xdg-open", psi.FileName);
                    break;
                case "MacOS":
                    Process.Start("open", psi.FileName);
                    break;
                default:
                    throw new PlatformNotSupportedException("Your platform is not supported.");
            }

            Process.Start(psi);
        }

        public void OnSettingsChange(object? send, AppSettings settings)
        {
            this.API = new(settings.APISettings);

            if (ShouldEnquire(settings))
                Enquire();
        }
    }
}

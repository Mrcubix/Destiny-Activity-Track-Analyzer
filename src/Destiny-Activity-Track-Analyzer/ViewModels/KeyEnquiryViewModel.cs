using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using API.Entities.User;
using Tracker.Shared;
using Tracker.Shared.Frontend;
using Tracker.Shared.Static;
using Tracker.Shared.Stores;

namespace Tracker.ViewModels
{
    public class KeyEnquiryViewModel : EnquiryViewModelBase
    {
        private ProcessStartInfo psi = new("https://mrcubix.github.io/Destiny-Activity-Track-Analyzer/Wiki/Setup#Creating-A-New-Bungie-Application")
        {
            UseShellExecute = true
        };

        public KeyEnquiryViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
        }

        public override bool ShouldEnquire(SharedStores stores)
        {
            if (!Remote.SharedStores.HasLoaded)
                return false;
                
            return !(SettingsStore.IsKeySet) && SettingsStore.Settings.UXSettings.ShouldEnquire;
        }

        public override void Enquire()
        {
            Remote.ShowView("Key Enquiry");
        }

        public override async Task Save()
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
                CurrentError = "";
                Remote.ShowView("User Enquiry");
            }
            else
            {
                CurrentError = "Error: Invalid API Key";
                ErrorColor = "Red";
            }
        }

        public override void Skip()
        {
            var defaultView = DefaultsStore.Defaults.DefaultViewModelName;

            if (defaultView != "")
                Remote.ShowView(defaultView);
            else
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
        }
    }
}

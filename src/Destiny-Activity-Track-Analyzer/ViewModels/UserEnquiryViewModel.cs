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
    public class UserEnquiryViewModel : EnquiryViewModelBase
    {
        private ProcessStartInfo psi = new("https://mrcubix.github.io/Destiny-Activity-Track-Analyzer/Wiki/Setup#Creating-A-New-Bungie-Application")
        {
            UseShellExecute = true
        };

        public UserEnquiryViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
        }

        public override bool ShouldEnquire(AppSettings settings)
        {
            bool keySet = SettingsStore.IsKeySet;

            return UserStore.User.UserInfo == null && keySet && settings.UXSettings.ShouldEnquire;
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
                    users = await API.SearchDestinyPlayerByBungieName(SettingsStore.Settings.APISettings.Username, SettingsStore.Settings.APISettings.Tag);
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
            
            if(users != null && users.Count > 0)
            {
                if (users.Count == 1)
                {
                    UserStore.User.UserInfo = users[0];
                    Remote.ShowView("Character Picker");
                }
                else
                {
                    // handle case where there are multiple platform?
                    // TODO: Check if this is an actual concern
                    UserStore.User.UserInfo = users[0];
                    Remote.ShowView("Character Picker");
                }

                // TODO: check if the character is set
                // if it is, then skip the rest of the Setup
                // if not, then go to 'Setup View'

                UserStore.Save();

                CurrentError = "";
            }
            else
            {
                CurrentError = "Error: Invalid Username or Tag (No users with such details found)";
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
    }
}

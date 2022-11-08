using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using API.Endpoints;
using API.Entities.Characters;
using API.Entities.Definitions;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using Tracker.Shared.Frontend;
using Tracker.Shared.Stores.Component;

namespace Tracker.ViewModels
{
    public class CurrentActivityViewModel : ViewModelBase
    {
        private const double second = 1000;
        private const double minute = second * 60;
        private const double hour = minute * 60;
        private const double day = hour * 24;

        private Definition<DestinyActivityDefinition> ActivityDefinitions;
        private Destiny2 API { get; set; }
        private DispatcherTimer timer = new() 
        {
            Interval = TimeSpan.FromMilliseconds(15)
        };


        private bool _isInOrbit = false;
        private DestinyCharacterComponent _currentCharacter = null!;
        private Bitmap? _currentModeIcon = null!;
        private DestinyActivityDefinition _currentActivity = null!;
        private DateTime _dateStarted = DateTime.Now;
        private string _timeElapsed = "";
        private bool _shouldShowTimer = false;
        
        
        public DestinyCharacterComponent CurrentCharacter
        {
            get => _currentCharacter;
            set => this.RaiseAndSetIfChanged(ref _currentCharacter, value);
        }

        public Bitmap? CurrentModeIcon
        {
            get => _currentModeIcon;
            set => this.RaiseAndSetIfChanged(ref _currentModeIcon, value);
        }

        public DestinyActivityDefinition CurrentActivity
        {
            get => _currentActivity;
            set => this.RaiseAndSetIfChanged(ref _currentActivity, value);
        }

        public DateTime DateStarted
        {
            get => _dateStarted;
            set => this.RaiseAndSetIfChanged(ref _dateStarted, value);
        }

        public string TimeElapsed
        {
            get => _timeElapsed;
            set => this.RaiseAndSetIfChanged(ref _timeElapsed, value);
        }

        public bool IsInOrbit 
        {
            get => _isInOrbit;
            set => this.RaiseAndSetIfChanged(ref _isInOrbit, value);
        }

        public bool ShouldShowTimer
        {
            get => _shouldShowTimer;
            set => this.RaiseAndSetIfChanged(ref _shouldShowTimer, value);
        }


        public CurrentActivityViewModel(ViewRemote remote, string name = "") : base(remote, name)
        {
            API = new(remote.SharedStores.SettingsStore.Settings.APISettings);
            ActivityDefinitions = Remote.SharedStores.DefinitionsStore.ActivityDefinitions;

            Initialize();
            _ = Task.Run(StartTrackingCurrentActivity);
        }


        public void StartTimer()
        {
            try
            {
                timer.Start();
                
                timer.Tick += delegate {
                    var elapsed = DateTime.Now - DateStarted;
                
                    TimeElapsed = elapsed.TotalMilliseconds switch
                    {
                        < minute => $"{elapsed.Seconds}.{elapsed.Milliseconds}",
                        >= minute and < hour => elapsed.ToString(@"mm\:ss\.fff"),
                        >= hour and < day => elapsed.ToString(@"hh\:mm\:ss\.fff"),
                        _ => elapsed.ToString(@"dd\:hh\:mm\:ss\.fff")
                    };
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Initialize()
        {
            // Note: The issue is not that it assign the value, but that the reference becomes absolete
            Remote.SharedStores.IconStore.IconsLoaded += OnIconsLoadComplete;
            Remote.SharedStores.SettingsStore.SettingsUpdated += OnSettingsChange;
            Remote.SharedStores.UserStore.CurrentCharacterChanged += OnCharacterChange;
        }

        public async Task StartTrackingCurrentActivity()
        {
            DestinyCharacterActivitiesComponent activity = new();

            while(true)
            {
                await Task.Delay(1000);

                if (CurrentCharacter != null && Remote.SharedStores.SettingsStore.IsKeySet)
                {
                    try
                    {
                        activity = await API.GetCurrentActivity(CurrentCharacter.MembershipType, CurrentCharacter.GetMembershipId(), CurrentCharacter.GetCharacterId());
                    }
                    catch(HttpRequestException)
                    {
                        Console.WriteLine("Failed to fetch current activity, Are you still connected to the internet?");
                    }

                    // TODO: Check if activity is either in ignore list or its main mode is enabled
                    if (activity.CurrentActivityHash != 0 && ActivityDefinitions.Items.ContainsKey(activity.CurrentActivityHash))
                    {
                        // We don't really want to replace the current activity if it's the same
                        if (CurrentActivity == null || CurrentActivity.Hash != activity.CurrentActivityHash)
                        {
                            // setup timer
                            DateStarted = activity.DateActivityStarted;
                            ShouldShowTimer = true;
                            StartTimer();

                            CurrentActivity = ActivityDefinitions.Items[activity.CurrentActivityHash];

                            // Some activities may not have an activity type (eg: Orbit)
                            if (CurrentActivity.directActivityModeType != null)
                                CurrentModeIcon = Remote.SharedStores.IconStore.ActivityIcons[CurrentActivity.directActivityModeType.Value];
                            else
                                CurrentModeIcon = Remote.SharedStores.IconStore.ActivityIcons[0];

                            // Orbit is Hash 82913930
                            IsInOrbit = CurrentActivity.Hash == 82913930;
                        }     
                    }   
                    else
                    {
                        // About the same thing as above
                        if (CurrentActivity != null || CurrentActivity?.Hash != 0)
                        {
                            timer.Stop();
                            ShouldShowTimer = false;

                            CurrentActivity = null!;
                            CurrentModeIcon = Remote.SharedStores.IconStore.ActivityIcons[0];
                            IsInOrbit = false;
                        }
                    }
                }
                else
                {
                    if (Remote.SharedStores.UserStore.User.Characters.Count != 0)
                        CurrentCharacter = Remote.SharedStores.DefaultsStore.Defaults.DefaultCharacter;
                }
            }
        }

        public void OnIconsLoadComplete(object? send, Dictionary<int, Bitmap?> icons)
        {
            CurrentModeIcon = icons[0];
        }

        public void OnSettingsChange(object? send, AppSettings settings)
        {
            this.API = new(settings.APISettings);
        }

        public void OnCharacterChange(object? sender, DestinyCharacterComponent character)
        {
            this.CurrentCharacter = character;
        }
    }
}

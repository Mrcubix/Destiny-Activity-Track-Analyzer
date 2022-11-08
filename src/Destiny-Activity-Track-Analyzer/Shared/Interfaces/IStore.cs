using System;

namespace Tracker.Shared.Interfaces
{
    public interface IStore
    {
        public bool HasLoaded { get; set; }

        
        void Load();

        void Initialize();

        void Save();

        //void Update(); // this is not ready yet
    }
}
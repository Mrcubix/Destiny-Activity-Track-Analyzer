using System;

namespace Tracker.Shared.Interfaces
{
    public interface IStore
    {
        void Load();

        void Initialize();

        void Save();

        //void Update(); // this is not ready yet
    }
}
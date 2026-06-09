using System;

namespace Jobworld
{
    public interface ILevelingModel
    {
        event Action<string> levelUpdated;
        event Action levelingObjectsChanged;
        ILevelingObject[] levelingObjects { get; }
    }
}
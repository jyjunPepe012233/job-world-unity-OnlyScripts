using System;

namespace Jobworld
{
    public interface ITutorialSelectionModel
    {
        JobSelectionPanelInfo info { get; }
        event Action<string> OnTutorialStart;
        void StartTutorial(string jobName);
    }
}
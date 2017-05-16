using System;

namespace Driver
{
    public class NotificationEventArgs : EventArgs
    {
        public int CurrentStep { get; }
        public int NumberOfSteps { get; }
        public ProgramService.EventType ActionType { get; }

        public NotificationEventArgs(int currentStep, int numberOfSteps, ProgramService.EventType actionType)
        {
            ActionType = actionType;
            CurrentStep = currentStep;
            NumberOfSteps = numberOfSteps;
            ActionType = actionType;
        }
    }
}

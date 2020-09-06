using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct StateChangedEventArgs : IGameEvent
    {
        public int CurrentState { get; }

        public StateChangedEventArgs(int currentState)
        {
            CurrentState = currentState;
        }
    }
}

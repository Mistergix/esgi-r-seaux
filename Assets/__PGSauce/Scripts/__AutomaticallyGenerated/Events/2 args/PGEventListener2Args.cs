using UnityEngine;
using UnityEngine.Events;
using PGSauce.Core.PGEvents;

namespace PGSauce.Core.PGEvents
{
    public class PGEventListener2Args<T0, T1, PGEventT, UnityEventT> : IPGEventListener where PGEventT : PGEvent2Args<T0, T1> where UnityEventT : UnityEvent<T0, T1>
    {
        [SerializeField] private PGEventT gameEvent;
        [SerializeField] private UnityEventT action;

        private void OnEnable()
        {
            gameEvent.Register(ActionCalled);
        }

        private void OnDisable()
        {
            gameEvent.Unregister(ActionCalled);
        }

        private void ActionCalled(T0 value0, T1 value1)
        {
            action.Invoke(value0, value1);
        }
    }
}
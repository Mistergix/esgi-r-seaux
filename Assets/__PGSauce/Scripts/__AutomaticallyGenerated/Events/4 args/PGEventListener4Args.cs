using UnityEngine;
using UnityEngine.Events;
using PGSauce.Core.PGEvents;

namespace PGSauce.Core.PGEvents
{
    public class PGEventListener4Args<T0, T1, T2, T3, PGEventT, UnityEventT> : IPGEventListener where PGEventT : PGEvent4Args<T0, T1, T2, T3> where UnityEventT : UnityEvent<T0, T1, T2, T3>
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

        private void ActionCalled(T0 value0, T1 value1, T2 value2, T3 value3)
        {
            action.Invoke(value0, value1, value2, value3);
        }
    }
}
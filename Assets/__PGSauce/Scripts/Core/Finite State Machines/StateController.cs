using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class StateController<T> : MonoBehaviour where T : StateController<T>
    {
        [SerializeField] private State<T> initialState;
        [SerializeField] private bool showDebug;

        private StateMachine<T> fsm;

        private State<T> CurrentState => fsm?.CurrentState;

        [ShowInInspector]
        private string CurrentStateName => CurrentState ? CurrentState.name : "";

        public bool ShowDebug => showDebug;

        private void Start()
        {
            fsm = new StateMachine<T>(initialState, this as T);
            Init();
        }

        protected virtual void Init()
        {
        }

        private void Update()
        {
            BeforeFsmUpdate();
            fsm.HandleInput();
            fsm.LogicUpdate();
            fsm.CheckTransitions();
            AfterFsmUpdate();
        }

        protected virtual void BeforeFsmUpdate()
        {
        }

        protected virtual void AfterFsmUpdate()
        {
        }

        private void FixedUpdate()
        {
            fsm.PhysicsUpdate();
        }
    }
}

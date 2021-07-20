using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.PGDebugging;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public class StateMachine<T> where T : StateController<T>
    {
        private State<T> currentState;
        private T controller;

        public State<T> CurrentState { get => currentState; private set => currentState = value; }
        private bool ShowDebug => controller.ShowDebug;

        public StateMachine(State<T> initialState, T controller)
        {
            this.CurrentState = initialState;
            this.controller = controller;
            CurrentState.OnEnter(controller);
        }

        public void ChangeState(State<T> newState)
        {
            if(! newState.IsNullState)
            {
                PGDebug.SetCondition(ShowDebug).Message($"Exiting {CurrentState.name}").Log();
                CurrentState.OnExit(controller);
                CurrentState = newState;
                PGDebug.SetCondition(ShowDebug).Message($"Entering {CurrentState.name}").Log();
                CurrentState.OnEnter(controller);
            }
        }

        public void HandleInput()
        {
            CurrentState.HandleInput(controller);
        }
        public void LogicUpdate()
        {
            CurrentState.LogicUpdate(controller);
        }
        public void PhysicsUpdate()
        {
            CurrentState.PhysicsUpdate(controller);
        }

        public void CheckTransitions()
        {
            CurrentState.CheckTransitions(controller, this);
        }
    }
}

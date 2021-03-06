//Copyright (c) 2021 APP ADVISORY SAS. All Rights Reserved // http://app-advisory.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PGSauce.Core.PGDebugging;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public class StateMachine<T> where T : StateMachineController<T>
    {
        public State<T> CurrentState { get; private set; }
        protected T Reference { get; private set; }
        private Dictionary<State<T>, List<Transition<T>>> allowedTransitions = new Dictionary<State<T>, List<Transition<T>>>();

        internal void CheckTransitions()
        {
            CurrentState.CheckTransitions(this);
        }

        public StateMachine(T reference)
        {
            Reference = reference;
        }

        public void AddNewTransition(State<T> from, State<T> to, Transition<T>.Decision decision)
        {
            var stateTransition = new Transition<T>(from, to, decision);

            IsKeyInAllowedTransitions(@from);

            allowedTransitions[from].Add(stateTransition);
        }

        private void IsKeyInAllowedTransitions(State<T> @from)
        {
            if (!allowedTransitions.ContainsKey(@from))
            {
                allowedTransitions.Add(@from, new List<Transition<T>>());
            }
        }

        public List<Transition<T>> GetTransitions(State<T> from)
        {
            IsKeyInAllowedTransitions(from);
            return allowedTransitions[from];
        }

        public void Initialize(State<T> initialState)
        {
            CurrentState = initialState;
            Enter();
        }

        public virtual void ChangeState(State<T> newState)
        {
                Exit();

                CurrentState = newState;

                Enter();
        }

        public virtual void Enter()
        {
            PGDebug.SetCondition(false).Message($"Entered state {CurrentState}").Log();
            CurrentState.Enter();
        }

        public virtual void HandleInput()
        {
            CurrentState.HandleInput();
        }

        public virtual void LogicUpdate()
        {
            CurrentState.LogicUpdate();
        }

        public virtual void PhysicsUpdate()
        {
            CurrentState.PhysicsUpdate();
        }

        public virtual void Exit()
        {
            PGDebug.SetCondition(false).Message($"Exited state {CurrentState}").Log();
            CurrentState.Exit();
        }
    }

}

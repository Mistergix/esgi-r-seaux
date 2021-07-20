//Copyright (c) 2021 APP ADVISORY SAS. All Rights Reserved // http://app-advisory.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public abstract class StateMachineController<T> : MonoBehaviour where T : StateMachineController<T>
    {
        [ShowInInspector, LabelText("Current State")]
        public string CurrentStateName => Fsm != null ? Fsm.CurrentState.ToString() : "";
        
        private StateMachine<T> fsm;

        public StateMachine<T> Fsm { get => fsm; private set => fsm = value; }

        private void Awake()
        {
            Fsm = new StateMachine<T>(this as T);
            InitFSM();
        }

        private void InitFSM()
        {
            InitializeStates();
            CreateTransitions();
            Fsm.Initialize(InitialState);
        }
        
        public void AddNewTransition(State<T> from, State<T> to, Transition<T>.Decision decision)
        {
            Fsm.AddNewTransition(from, to, decision);
        }

        protected abstract void InitializeStates();

        protected abstract void CreateTransitions();

        public abstract State<T> InitialState { get; }

        private void Update()
        {
            Fsm.HandleInput();
            Fsm.LogicUpdate();
            Fsm.CheckTransitions();
        }

        private void FixedUpdate()
        {
            Fsm.PhysicsUpdate();
        }
    }
}

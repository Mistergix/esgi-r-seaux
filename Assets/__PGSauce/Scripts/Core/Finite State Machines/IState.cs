using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class IState : ScriptableObject
    {
        [SerializeField][HideIf("IsNullState")] private string stateName;
        [SerializeField] private bool isNullState;

        public string StateName => IsNullState ? "NULL" : (stateName == "" ? name : stateName);
        public bool IsNullState => isNullState;

        public abstract List<ITransition> GetTransitions();
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.Utilities
{
    public abstract class Selector<T> : MonoBehaviour
    {
        private int _currentIndex;

        public T CurrentObject => Objects[_currentIndex];

        protected abstract List<T> Objects
        {
            get;
            set;
        }

        private void Awake()
        {
            OnAwake();
        }

        protected abstract bool Cyclic { get; }

        public int CurrentIndex => _currentIndex;

        protected virtual void OnPreviousObject(){}
        protected virtual void OnNextObject(){}

        protected virtual void OnObjectChanged(){}

        protected abstract void OnAwake();

        public void NextObject()
        {
            _currentIndex++;
            if (Cyclic)
            {
                _currentIndex %= Objects.Count;
            }
            else
            {
                _currentIndex = Mathf.Clamp(_currentIndex,0, Objects.Count - 1);
            }
            
            OnNextObject();
            OnObjectChanged();
        }

        public void PreviousObject()
        {
            _currentIndex--;
            if (Cyclic)
            {
                if (_currentIndex < 0)
                {
                    _currentIndex += Objects.Count;
                }
            }
            else
            {
                _currentIndex = Mathf.Clamp(_currentIndex,0, Objects.Count - 1);
            }
            
            OnPreviousObject();
            OnObjectChanged();
        }

    }
}

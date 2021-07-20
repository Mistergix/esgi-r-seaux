using System;
using UnityEngine;
using PGSauce.Core.PGEvents;

namespace PGSauce.Core.PGEvents
{
	public class PGEvent3Args<T0, T1, T2> : ScriptableObject
	{
		private event Action<T0, T1, T2> action;

		public void Raise(T0 value0, T1 value1, T2 value2)
		{
			action?.Invoke(value0, value1, value2);
		}

		public void Register(Action<T0, T1, T2> callback)
		{
			action += callback;
		}

		public void Unregister(Action<T0, T1, T2> callback)
		{
			action -= callback;
		}
	}
}

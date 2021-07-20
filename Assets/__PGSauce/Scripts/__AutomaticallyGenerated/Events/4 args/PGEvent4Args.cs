using System;
using UnityEngine;
using PGSauce.Core.PGEvents;

namespace PGSauce.Core.PGEvents
{
	public class PGEvent4Args<T0, T1, T2, T3> : ScriptableObject
	{
		private event Action<T0, T1, T2, T3> action;

		public void Raise(T0 value0, T1 value1, T2 value2, T3 value3)
		{
			action?.Invoke(value0, value1, value2, value3);
		}

		public void Register(Action<T0, T1, T2, T3> callback)
		{
			action += callback;
		}

		public void Unregister(Action<T0, T1, T2, T3> callback)
		{
			action -= callback;
		}
	}
}

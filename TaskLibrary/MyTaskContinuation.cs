using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	internal class MyTaskContinuation : MyTask
	{
		private MyTask _antecedent;
		private Action<MyTask> _action;

		public MyTaskContinuation(Action<MyTask> continuation, MyTask antecedent)
		{
			_action = continuation;
			_antecedent = antecedent;
		}

		internal void Invoke()
		{
			_action(_antecedent);
			IsCompleted = true;
		}
	}
}

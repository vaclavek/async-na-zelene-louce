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

		public MyTaskContinuation(Action<MyTask> continuation, MyTask antecedent, MyTaskScheduler scheduler = null)
			: base(scheduler ?? MyTaskScheduler.Default)
		{
			_action = continuation;
			_antecedent = antecedent;
		}

		internal override void Invoke()
		{
			_action(_antecedent);
			IsCompleted = true;
		}
	}

	internal class MyTaskContinuation<T> : MyTask<T>
	{
		private MyTask _antecedent;
		private Func<MyTask, T> _action;

		public MyTaskContinuation(Func<MyTask, T> continuation, MyTask antecedent, MyTaskScheduler scheduler = null)
			: base(scheduler ?? MyTaskScheduler.Default)
		{
			_action = continuation;
			_antecedent = antecedent;
		}

		internal override void Invoke()
		{
			Result = _action(_antecedent);
			IsCompleted = true;
		}
	}
}

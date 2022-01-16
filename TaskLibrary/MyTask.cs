using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	public class MyTask
	{
		internal MyTask()
		{
		}

		private bool _isCompleted;
		public bool IsCompleted
		{
			get => _isCompleted;
			internal set
			{
				_isCompleted = value;

				if (value)
				{
					_mutex.Set();
				}
			}
		}

		private readonly ManualResetEventSlim _mutex = new();

		public void Wait()
		{
			_mutex.Wait();
		}

		public MyTask ContinueWith(Action<MyTask> continuation)
		{
			var task = new MyTaskContinuation(continuation, this);

			// AddContinuation(task);

			return task;
		}

		public MyTask<T> ContinueWith<T>(Func<MyTask, T> continuation)
		{
			var task = new MyTaskContinuation<T>(continuation, this);

			// AddContinuation(task);

			return task;
		}
	}

	public class MyTask<T> : MyTask
	{
		public T Result { get; internal set; }
	}
}

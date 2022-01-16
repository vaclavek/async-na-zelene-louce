using System;
using System.Collections.Concurrent;
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
					InvokeContinuations();
				}
			}
		}

		private readonly ManualResetEventSlim _mutex = new();

		public void Wait()
		{
			_mutex.Wait();
		}

		private readonly ConcurrentQueue<MyTask> _continuations = new();
		public MyTask ContinueWith(Action<MyTask> continuation)
		{
			var task = new MyTaskContinuation(continuation, this);

			AddContinuation(task);

			return task;
		}

		public MyTask<T> ContinueWith<T>(Func<MyTask, T> continuation)
		{
			var task = new MyTaskContinuation<T>(continuation, this);

			AddContinuation(task);

			return task;
		}

		private protected void AddContinuation(MyTask continuation)
		{
			// todo: race condition
			if (IsCompleted)
			{
				continuation.Invoke();
				return;
			}
			_continuations.Enqueue(continuation);
		}

		private void InvokeContinuations()
		{
			while (_continuations.TryDequeue(out MyTask continuation))
			{
				continuation.Invoke();
			}
		}

		internal virtual void Invoke()
		{
			throw new InvalidOperationException();
		}
	}

	public class MyTask<T> : MyTask
	{
		public T Result { get; internal set; }

		public MyTask ContinueWith(Action<MyTask<T>> continuation)
		{
			var task = new MyTaskContinuation(f => continuation((MyTask<T>)f), this);

			AddContinuation(task);

			return task;
		}

		public MyTask<TResult> ContinueWith<TResult>(Func<MyTask<T>, TResult> continuation)
		{
			var task = new MyTaskContinuation<TResult>(f => continuation((MyTask<T>)f), this);

			AddContinuation(task);

			return task;
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	public class MyTask
	{
		internal MyTask(MyTaskScheduler scheduler = null)
		{
			Scheduler = scheduler;
		}

		public MyTaskScheduler Scheduler { get; }

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
		public MyTask ContinueWith(Action<MyTask> continuation, MyTaskScheduler scheduler = null)
		{
			var task = new MyTaskContinuation(continuation, this, scheduler);

			AddContinuation(task);

			return task;
		}

		public MyTask<T> ContinueWith<T>(Func<MyTask, T> continuation, MyTaskScheduler scheduler = null)
		{
			var task = new MyTaskContinuation<T>(continuation, this, scheduler);

			AddContinuation(task);

			return task;
		}

		private protected void AddContinuation(MyTask continuation)
		{
			// todo: race condition
			if (IsCompleted)
			{
				continuation.ScheduleAndStart();
				return;
			}
			_continuations.Enqueue(continuation);
		}

		private void InvokeContinuations()
		{
			if (_continuations.Count == 1)
			{
				_continuations.TryDequeue(out var continuation);
				if (!continuation.Scheduler.TryExecuteTaskInline(continuation))
				{
					continuation.ScheduleAndStart();
				}
				return;
			}

			while (_continuations.TryDequeue(out MyTask continuation))
			{
				continuation.ScheduleAndStart();
			}
		}

		private void ScheduleAndStart()
		{
			Scheduler.QueueTask(this);
		}

		internal virtual void Invoke()
		{
			throw new InvalidOperationException();
		}
	}

	[AsyncMethodBuilder(typeof(MyTaskBuilder<>))]
	public class MyTask<T> : MyTask
	{
		public T Result { get; internal set; }

		internal MyTask(MyTaskScheduler scheduler = null)
			: base(scheduler)
		{
		}

		public MyTask ContinueWith(Action<MyTask<T>> continuation, MyTaskScheduler scheduler = null)
		{
			var task = new MyTaskContinuation(f => continuation((MyTask<T>)f), this, scheduler);

			AddContinuation(task);

			return task;
		}

		public MyTask<TResult> ContinueWith<TResult>(Func<MyTask<T>, TResult> continuation, MyTaskScheduler scheduler = null)
		{
			var task = new MyTaskContinuation<TResult>(f => continuation((MyTask<T>)f), this, scheduler);

			AddContinuation(task);

			return task;
		}
	}
}

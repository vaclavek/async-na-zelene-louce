using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	public interface IAwaiter<T>
	{
		T GetResult();
		bool IsCompleted { get; }
		void OnCompleted(Action action);
	}

	public struct MyTaskAwaiter<T> : IAwaiter<T>
	{
		private readonly MyTask<T> _task;

		public MyTaskAwaiter(MyTask<T> task)
		{
			_task = task;
		}

		public T GetResult() => _task.Result;

		public bool IsCompleted => _task.IsCompleted;

		public void OnCompleted(Action action)
		{
			_task.ContinueWith(_ => action());
		}
	}

	public static class MyTaskExtensions
	{
		public static MyTaskAwaiter<T> GetAwaiter<T>(this MyTask<T> task)
		{
			return new MyTaskAwaiter<T>(task);
		}
	}

	public class Api
	{
		public static void DisplayCurrentSynchronizationContext()
		{
			Console.WriteLine(SynchronizationContext.Current?.ToString() ?? "No synchronization context");
		}

		//public async MyTask<int> CallAsync()
		//{
		//	// sync code
		//	int i = await DoSomethingAsync1();
		//	int j = await DoSomethingAsync2();
		//	int k = await DoSomethingAsync3();

		//	return await DoSomethingElseAsync(i + j + k);
		//}

		public MyTask<int> CallAsync()
		{
			var stateMachine = new StateMachine();
			stateMachine.This = this;
			stateMachine.MoveNext();
			return stateMachine.Builder.Task;
		}

		class StateMachine
		{
			public Api This;

			public MyTaskCompletionSource<int> Builder = new();

			private int _state;
			private MyTask<int> _awaiter;

			private int _i;
			private int _j;
			private int _k;

			public void MoveNext()
			{
				switch(_state)
				{
					case 0:
						{
							_state = 1;
							// sync code
							_awaiter = This.DoSomethingAsync1();

							if(_awaiter.IsCompleted)
							{
								goto case 1;
							}
							_awaiter.ContinueWith(_ => MoveNext());

							return;
						}

					case 1:
						{
							_state = 2;

							_i = _awaiter.Result;

							_awaiter = This.DoSomethingAsync2();
							if (_awaiter.IsCompleted)
							{
								goto case 2;
							}
							_awaiter.ContinueWith(_ => MoveNext());

							return;
						}

					case 2:
						{
							_state = 3;

							_j = _awaiter.Result;

							_awaiter = This.DoSomethingAsync3();
							if (_awaiter.IsCompleted)
							{
								goto case 3;
							}
							_awaiter.ContinueWith(_ => MoveNext());

							return;
						}

					case 3:
						{
							_state = 4;

							_k = _awaiter.Result;

							_awaiter = This.DoSomethingElseAsync(_i + _j + _k);
							if (_awaiter.IsCompleted)
							{
								goto case 4;
							}
							_awaiter.ContinueWith(_ => MoveNext());

							return;
						}

					case 4:
						{
							Builder.Complete(_awaiter.Result);
							return;
						}


				}
			}
		}

		private MyTask<int> DoSomethingAsync1()
		{
			return Delay(500).ContinueWith(_ => 1);
		}

		private MyTask<int> DoSomethingAsync2()
		{
			return Delay(500).ContinueWith(_ => 10);
		}

		private MyTask<int> DoSomethingAsync3()
		{
			return Delay(500).ContinueWith(_ => 100);
		}

		public MyTask<int> DoSomethingElseAsync(int input)
		{
			return Delay(500).ContinueWith(_ => input * 2);
		}

		public static MyTask Delay(int ms)
		{
			var tcs = new MyTaskCompletionSource();

			var thread = new Thread(() =>
			{
				Thread.Sleep(ms);
				tcs.Complete();
			});
			thread.Start();
			return tcs.Task;
		}
	}
}

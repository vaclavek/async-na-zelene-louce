using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	public class MyTaskCompletionSource
	{
		public MyTask Task { get; }

		public MyTaskCompletionSource()
		{
			Task = new();
		}

		public void Complete()
		{
			Task.IsCompleted = true;
		}
	}
}

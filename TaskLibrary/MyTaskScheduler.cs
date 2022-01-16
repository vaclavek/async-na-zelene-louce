using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLibrary
{
	public abstract class MyTaskScheduler
	{
		public static readonly MyTaskScheduler Default = new MyThreadPoolTaskScheduler();
		protected internal abstract void QueueTask(MyTask task);
		protected void ExecuteTask(MyTask task)
		{
			if (task.Scheduler != this)
			{
				throw new NotSupportedException();
			}
			task.Invoke();
		}
	}

	public class MyThreadPoolTaskScheduler : MyTaskScheduler
	{
		protected internal override void QueueTask(MyTask task)
		{
			ThreadPool.QueueUserWorkItem(_ => ExecuteTask(task), null);
		}
	}
}

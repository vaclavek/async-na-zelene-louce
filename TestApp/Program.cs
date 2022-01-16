using TaskLibrary;

Console.WriteLine("Start");

var task = AsyncMethod();
task.Wait();

Console.WriteLine("Task completed with value " + task.Result);
Console.WriteLine("Done");

static MyTask<int> AsyncMethod()
{
	var tcs = new MyTaskCompletionSource<int>();

	var thread = new Thread(() =>
	{
		Thread.Sleep(1000);
		tcs.Complete(42);
	});

	thread.Start();

	return tcs.Task;
}
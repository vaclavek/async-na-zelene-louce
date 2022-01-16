using TaskLibrary;

Console.WriteLine("Start");

var task = AsyncMethod();
task.Wait();

Console.WriteLine("Done");

static MyTask AsyncMethod()
{
	var tcs = new MyTaskCompletionSource();

	var thread = new Thread(() =>
	{
		Thread.Sleep(1000);
		tcs.Complete();
	});

	thread.Start();

	return tcs.Task;
}
using TaskLibrary;

Console.WriteLine("Start");

//var task = AsyncMethod();

//task.ContinueWith((t) =>
//{
//	Thread.Sleep(2000);

//	Console.WriteLine("Task completed with value " + t.Result);
//	return t.Result * 2;
//}).ContinueWith((t2) =>
//{
//	Console.WriteLine("Second task completed with value " + t2.Result);
//});

//task.ContinueWith((t) =>
//{
//	Console.WriteLine("Second continuation");
//});

//Console.WriteLine("Done");
//Console.ReadLine();

//static MyTask<int> AsyncMethod()
//{
//	var tcs = new MyTaskCompletionSource<int>();

//	var thread = new Thread(() =>
//	{
//		Thread.Sleep(1000);
//		tcs.Complete(42);
//	});

//	thread.Start();

//	return tcs.Task;
//}

//var task = new Api().CallAsync();
//task.Wait();

//Console.WriteLine(task.Result);

var sc = new SingleThreadedSynchronizationContext();
sc.Post(_ => new Api().CallAsync(), null);

Console.WriteLine("Done");
Console.ReadLine();
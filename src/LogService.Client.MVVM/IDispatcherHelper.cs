using System;
using System.Threading.Tasks;

namespace LogService.Client.MVVM;

public interface IDispatcherHelper
{
	void BeginInvokeOnMainThread(Action action);
	Task BeginInvokeOnMainThread(Func<Task> action);
}
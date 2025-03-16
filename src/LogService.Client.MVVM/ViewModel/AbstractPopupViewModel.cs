using System;

namespace LogService.Client.MVVM.ViewModel;

public interface IPopupViewModel { }

public abstract class AbstractPopupViewModel<T> : AbstractViewModel, IPopupViewModel
{
	public Action<T> SetResult { get; set; }
}
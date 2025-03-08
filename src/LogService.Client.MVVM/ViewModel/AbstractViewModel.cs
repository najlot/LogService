using System.Collections.Generic;
using System.ComponentModel;

namespace LogService.Client.MVVM.ViewModel;

public abstract class AbstractViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public virtual bool Set<T>(string propertyName, ref T oldValue, T newValue)
	{
		if (EqualityComparer<T>.Default.Equals(oldValue, default) &&
			EqualityComparer<T>.Default.Equals(newValue, default))
		{
			return false;
		}

		if (oldValue?.Equals(newValue) ?? false)
		{
			return false;
		}

		oldValue = newValue;
		RaisePropertyChanged(propertyName);

		return true;
	}
}